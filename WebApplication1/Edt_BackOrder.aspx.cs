using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Edt_BackOrder : System.Web.UI.Page
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


        // Fill Item Dropdown (only items of the supplier of this back order's PO)
        void fillItemCombo()
        {
            getCon();

            da = new SqlDataAdapter(
                "SELECT Item_Id, Item_Name FROM Item_Tbl WHERE Item_Status = 1 " +
                "AND Item_SupplierId = (SELECT p.PO_SupplierId FROM BackOrder_Tbl b INNER JOIN PO_Tbl p ON b.BO_PO_Id = p.PO_Id " +
                "WHERE b.BO_Id = '" + Request.QueryString["id"] + "') ORDER BY Item_Name ASC",
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
            if (ViewState["BOItems"] == null)
            {
                ViewState["BOItems"] = newItemsTable();
            }
            return (DataTable)ViewState["BOItems"];
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


        protected void drpitem_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillPrice();
        }


        // Add current item selection into the cart
        protected void btn_additem_Click(object sender, EventArgs e)
        {
            if (drpitem.Items.Count == 0)
            {
                Response.Write("<script>alert('There are no items for this supplier')</script>");
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

            ViewState["BOItems"] = dt;

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

                ViewState["BOItems"] = dt;

                bindItemsGrid();
            }
        }



        // Load Back Order header + existing item lines
        void fillData()
        {
            getCon();

            // Header
            da = new SqlDataAdapter(
                "select b.BO_PO_Id, b.BO_Remarks, b.BO_Status, s.Supp_Name " +
                "from BackOrder_Tbl b " +
                "inner join PO_Tbl p on b.BO_PO_Id = p.PO_Id " +
                "inner join Supplier_Tbl s on p.PO_SupplierId = s.Supp_Id " +
                "where b.BO_Id = '" + Request.QueryString["id"] + "'",
                con
            );
            ds = new DataSet();
            da.Fill(ds);

            lblpo.Text = "PO #" + ds.Tables[0].Rows[0]["BO_PO_Id"].ToString() + " - " + ds.Tables[0].Rows[0]["Supp_Name"].ToString();
            txtremarks.Text = ds.Tables[0].Rows[0]["BO_Remarks"].ToString();
            drpstatus.SelectedValue = ds.Tables[0].Rows[0]["BO_Status"].ToString();

            // Existing item lines
            da = new SqlDataAdapter(
                "select boi.BOI_ItemId, i.Item_Name, boi.BOI_Qty, boi.BOI_Price, boi.BOI_Amount " +
                "from BO_Items_Tbl boi inner join Item_Tbl i on boi.BOI_ItemId = i.Item_Id " +
                "where boi.BOI_BO_Id = '" + Request.QueryString["id"] + "'",
                con
            );

            DataSet itemsDs = new DataSet();
            da.Fill(itemsDs);

            DataTable dt = newItemsTable();

            foreach (DataRow r in itemsDs.Tables[0].Rows)
            {
                DataRow row = dt.NewRow();
                row["ItemId"] = r["BOI_ItemId"].ToString();
                row["ItemName"] = r["Item_Name"].ToString();
                row["Qty"] = r["BOI_Qty"].ToString();
                row["Price"] = Convert.ToDecimal(r["BOI_Price"]).ToString("0.00");
                row["Amount"] = Convert.ToDecimal(r["BOI_Amount"]).ToString("0.00");
                dt.Rows.Add(row);
            }

            ViewState["BOItems"] = dt;

            bindItemsGrid();

            con.Close();
        }


        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("BackOrder.aspx");
        }


        // Update Back Order (header + replace item lines + stock)
        protected void edt_bo_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = getItemsTable();

            if (dt.Rows.Count == 0)
            {
                Response.Write("<script>alert('Please add at least one item to the back order')</script>");
                return;
            }

            decimal total = 0;
            foreach (DataRow r in dt.Rows)
            {
                total += Convert.ToDecimal(r["Amount"]);
            }

            getCon();

            // Update Back Order header
            cmd = new SqlCommand(
                "update BackOrder_Tbl set BO_Remarks='" + txtremarks.Text + "', BO_TotalAmount='" + total + "', " +
                "BO_Status='" + drpstatus.SelectedValue + "' where BO_Id='" + Request.QueryString["id"] + "'",
                con
            );
            cmd.ExecuteNonQuery();

            // Remove old item lines
            cmd = new SqlCommand(
                "delete from BO_Items_Tbl where BOI_BO_Id = '" + Request.QueryString["id"] + "'",
                con
            );
            cmd.ExecuteNonQuery();

            // Re-insert current item lines
            foreach (DataRow r in dt.Rows)
            {
                cmd = new SqlCommand(
                    "INSERT INTO BO_Items_Tbl (BOI_BO_Id, BOI_ItemId, BOI_Qty, BOI_Price, BOI_Amount) " +
                    "VALUES ('" + Request.QueryString["id"] + "', '" + r["ItemId"] + "', '" + r["Qty"] + "', '" + r["Price"] + "', '" + r["Amount"] + "')",
                    con
                );
                cmd.ExecuteNonQuery();
            }

            // Stock: always clear this back order's stock rows first,
            // then add them again only when the status is Received
            cmd = new SqlCommand(
                "delete from Stock_Tbl where Stock_Ref = 'BO-" + Request.QueryString["id"] + "'",
                con
            );
            cmd.ExecuteNonQuery();

            if (drpstatus.SelectedValue == "Received")
            {
                foreach (DataRow r in dt.Rows)
                {
                    cmd = new SqlCommand(
                        "INSERT INTO Stock_Tbl (Stock_ItemId, Stock_Qty, Stock_Type, Stock_Ref) " +
                        "VALUES ('" + r["ItemId"] + "', '" + r["Qty"] + "', 'IN', 'BO-" + Request.QueryString["id"] + "')",
                        con
                    );
                    cmd.ExecuteNonQuery();
                }
            }

            Response.Write("<script>alert('Back Order Updated Successfully')</script>");
            Response.Redirect("BackOrder.aspx");
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
                fillItemCombo();
                fillPrice();
                fillData();
            }
        }
    }
}
