using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Edt_PurchaseOrder : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter da;
        DataSet ds;

        string s = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;


        // Connection
        void getCon()
        {
            con = new SqlConnection(s);
            con.Open();
        }


        // Fill Supplier Dropdown
        void fillSupplierCombo()
        {
            getCon();

            da = new SqlDataAdapter(
                "SELECT Supp_Id, Supp_Name FROM Supplier_Tbl WHERE Supp_Status = 1 ORDER BY Supp_Name ASC",
                con
            );

            ds = new DataSet();
            da.Fill(ds);

            drpsupplier.DataSource = ds.Tables[0];
            drpsupplier.DataTextField = "Supp_Name";
            drpsupplier.DataValueField = "Supp_Id";
            drpsupplier.DataBind();

            con.Close();
        }


        // Fill Item Dropdown
        void fillItemCombo()
        {
            getCon();

            da = new SqlDataAdapter(
                "SELECT Item_Id, Item_Name FROM Item_Tbl WHERE Item_Status = 1 " +
                "AND Item_SupplierId = '" + drpsupplier.SelectedValue + "' ORDER BY Item_Name ASC",
                con
            );

            ds = new DataSet();
            da.Fill(ds);

            drpitem.DataSource = ds.Tables[0];
            drpitem.DataTextField = "Item_Name";
            drpitem.DataValueField = "Item_Id";
            drpitem.DataBind();

            con.Close();
        }


        // Build an empty items table (cart) with the columns we need
        DataTable newItemsTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemId");
            dt.Columns.Add("ItemName");
            dt.Columns.Add("Qty");
            dt.Columns.Add("Price");
            dt.Columns.Add("Amount");
            return dt;
        }


        // Get the items cart from ViewState
        DataTable getItemsTable()
        {
            if (ViewState["POItems"] == null)
            {
                ViewState["POItems"] = newItemsTable();
            }
            return (DataTable)ViewState["POItems"];
        }


        // Rebind the items grid + total
        void bindItemsGrid()
        {
            DataTable dt = getItemsTable();

            GridView2.DataSource = dt;
            GridView2.DataBind();

            decimal total = 0;
            foreach (DataRow r in dt.Rows)
            {
                total += Convert.ToDecimal(r["Amount"]);
            }
            lbltotal.Text = total.ToString("0.00");
        }


        // Load PO header + existing item lines
        void fillData()
        {
            getCon();

            // Header
            da = new SqlDataAdapter(
                "select * from PO_Tbl where PO_Id = '" + Request.QueryString["id"] + "'",
                con
            );
            ds = new DataSet();
            da.Fill(ds);

            drpsupplier.SelectedValue = ds.Tables[0].Rows[0]["PO_SupplierId"].ToString();
            txtremarks.Text = ds.Tables[0].Rows[0]["PO_Remarks"].ToString();
            drpstatus.SelectedValue = ds.Tables[0].Rows[0]["PO_Status"].ToString();

            // Existing item lines
            da = new SqlDataAdapter(
                "select poi.POI_ItemId, i.Item_Name, poi.POI_Qty, poi.POI_Price, poi.POI_Amount " +
                "from PO_Items_Tbl poi inner join Item_Tbl i on poi.POI_ItemId = i.Item_Id " +
                "where poi.POI_PO_Id = '" + Request.QueryString["id"] + "'",
                con
            );

            DataSet itemsDs = new DataSet();
            da.Fill(itemsDs);

            DataTable dt = newItemsTable();

            foreach (DataRow r in itemsDs.Tables[0].Rows)
            {
                DataRow row = dt.NewRow();
                row["ItemId"] = r["POI_ItemId"].ToString();
                row["ItemName"] = r["Item_Name"].ToString();
                row["Qty"] = r["POI_Qty"].ToString();
                row["Price"] = Convert.ToDecimal(r["POI_Price"]).ToString("0.00");
                row["Amount"] = Convert.ToDecimal(r["POI_Amount"]).ToString("0.00");
                dt.Rows.Add(row);
            }

            ViewState["POItems"] = dt;

            bindItemsGrid();
        }


        // Put the cost price of the selected item into the price box (can still be changed)
        void fillPrice()
        {
            if (drpitem.Items.Count == 0)
            {
                txtprice.Text = "";
                return;
            }

            getCon();

            cmd = new SqlCommand("select Item_Cost from Item_Tbl where Item_Id = '" + drpitem.SelectedValue + "'", con);

            txtprice.Text = Convert.ToDecimal(cmd.ExecuteScalar()).ToString("0.00");

            con.Close();
        }


        protected void drpsupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillItemCombo();
            fillPrice();

            // items of another supplier cannot stay in this list
            ViewState["POItems"] = newItemsTable();
            bindItemsGrid();
        }


        protected void drpitem_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillPrice();
        }


        // Add current item selection into the cart
        protected void btn_additem_Click(object sender, EventArgs e)
        {
            if (drpitem.Items.Count == 0)
            {
                Response.Write("<script>alert('There are no items for the selected supplier')</script>");
                return;
            }

            if (string.IsNullOrEmpty(txtqty.Text) || string.IsNullOrEmpty(txtprice.Text))
            {
                Response.Write("<script>alert('Please enter Qty and Price')</script>");
                return;
            }

            int qty = Convert.ToInt32(txtqty.Text);
            decimal price = Convert.ToDecimal(txtprice.Text);
            decimal amount = qty * price;

            DataTable dt = getItemsTable();

            DataRow row = dt.NewRow();
            row["ItemId"] = drpitem.SelectedValue;
            row["ItemName"] = drpitem.SelectedItem.Text;
            row["Qty"] = qty;
            row["Price"] = price.ToString("0.00");
            row["Amount"] = amount.ToString("0.00");

            dt.Rows.Add(row);

            ViewState["POItems"] = dt;

            bindItemsGrid();

            txtqty.Text = "";
            fillPrice();
        }


        // Remove a line from the cart
        protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "cmd_rmv")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                DataTable dt = getItemsTable();
                dt.Rows.RemoveAt(index);

                ViewState["POItems"] = dt;

                bindItemsGrid();
            }
        }


        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("PurchaseOrder.aspx");
        }


        // Update Purchase Order (header + replace item lines)
        protected void edt_po_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = getItemsTable();

            if (dt.Rows.Count == 0)
            {
                Response.Write("<script>alert('Please add at least one item to the order')</script>");
                return;
            }

            decimal total = 0;
            foreach (DataRow r in dt.Rows)
            {
                total += Convert.ToDecimal(r["Amount"]);
            }

            getCon();

            // Update PO header
            cmd = new SqlCommand(
                "update PO_Tbl set PO_SupplierId='" + drpsupplier.SelectedValue + "', PO_Remarks='" + txtremarks.Text + "', " +
                "PO_TotalAmount='" + total + "', PO_Status='" + drpstatus.SelectedValue + "' where PO_Id='" + Request.QueryString["id"] + "'",
                con
            );
            cmd.ExecuteNonQuery();

            // Remove old item lines
            cmd = new SqlCommand(
                "delete from PO_Items_Tbl where POI_PO_Id = '" + Request.QueryString["id"] + "'",
                con
            );
            cmd.ExecuteNonQuery();

            // Re-insert current item lines
            foreach (DataRow r in dt.Rows)
            {
                cmd = new SqlCommand(
                    "INSERT INTO PO_Items_Tbl (POI_PO_Id, POI_ItemId, POI_Qty, POI_Price, POI_Amount) " +
                    "VALUES ('" + Request.QueryString["id"] + "', '" + r["ItemId"] + "', '" + r["Qty"] + "', '" + r["Price"] + "', '" + r["Amount"] + "')",
                    con
                );
                cmd.ExecuteNonQuery();
            }

            Response.Write("<script>alert('Purchase Order Updated Successfully')</script>");
            Response.Redirect("PurchaseOrder.aspx");
        }


        // Page Load
        protected void Page_Load(object sender, EventArgs e)
        {
            // Login Check
            if (Session["admin"] == null)
            {
                Response.Redirect("login.aspx");
            }

            // First Page Load
            if (!IsPostBack)
            {
                getCon();
                cmd = new SqlCommand("select PO_Status from PO_Tbl where PO_Id = '" + Request.QueryString["id"] + "'", con);
                string poStatus = cmd.ExecuteScalar().ToString();
                con.Close();

                if (poStatus == "Received")
                {
                    Response.Write("<script>alert('This Purchase Order is already received and cannot be edited');window.location='PurchaseOrder.aspx';</script>");
                    return;
                }

                fillSupplierCombo();
                fillData();
                fillItemCombo();
                fillPrice();
            }
        }
    }
}
