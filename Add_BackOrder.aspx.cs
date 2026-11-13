using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Add_BackOrder : System.Web.UI.Page
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


        // Fill Purchase Order Dropdown:
        // POs that were received, still have missing items, and have no back order yet
        void fillPOCombo()
        {
            getCon();

            da = new SqlDataAdapter(
                "SELECT p.PO_Id, s.Supp_Name FROM PO_Tbl p " +
                "INNER JOIN Supplier_Tbl s ON p.PO_SupplierId = s.Supp_Id " +
                "WHERE p.PO_Status = 'Received' " +
                "AND NOT EXISTS (SELECT 1 FROM BackOrder_Tbl b WHERE b.BO_PO_Id = p.PO_Id) " +
                "AND (SELECT ISNULL(SUM(POI_Qty), 0) FROM PO_Items_Tbl WHERE POI_PO_Id = p.PO_Id) > " +
                "(SELECT ISNULL(SUM(ISNULL(POI_RecvQty, POI_Qty)), 0) FROM PO_Items_Tbl WHERE POI_PO_Id = p.PO_Id) " +
                "ORDER BY p.PO_Id DESC",
                con
            );

            ds = new DataSet();
            da.Fill(ds);

            // Build a friendly display text: "PO #3 - ABC Supplier"
            ds.Tables[0].Columns.Add("PODisplay", typeof(string));
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                r["PODisplay"] = "PO #" + r["PO_Id"].ToString() + " - " + r["Supp_Name"].ToString();
            }

            drppo.DataSource = ds.Tables[0];
            drppo.DataTextField = "PODisplay";
            drppo.DataValueField = "PO_Id";
            drppo.DataBind();

            con.Close();
        }


        // Fill Item Dropdown (only items of the supplier of the selected PO)
        void fillItemCombo()
        {
            getCon();

            da = new SqlDataAdapter(
                "SELECT Item_Id, Item_Name FROM Item_Tbl WHERE Item_Status = 1 " +
                "AND Item_SupplierId = (SELECT PO_SupplierId FROM PO_Tbl WHERE PO_Id = '" + drppo.SelectedValue + "') ORDER BY Item_Name ASC",
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


        // Fill the cart with the missing items of the selected PO (ordered qty - received qty)
        void fillPending()
        {
            DataTable cart = newItemsTable();

            if (drppo.Items.Count > 0)
            {
                getCon();

                da = new SqlDataAdapter(
                    "select poi.POI_ItemId, i.Item_Name, " +
                    "sum(poi.POI_Qty) - sum(isnull(poi.POI_RecvQty, poi.POI_Qty)) as Qty, max(poi.POI_Price) as Price " +
                    "from PO_Items_Tbl poi inner join Item_Tbl i on poi.POI_ItemId = i.Item_Id " +
                    "where poi.POI_PO_Id = '" + drppo.SelectedValue + "' " +
                    "group by poi.POI_ItemId, i.Item_Name",
                    con
                );
                DataSet missingDs = new DataSet();
                da.Fill(missingDs);

                con.Close();

                foreach (DataRow m in missingDs.Tables[0].Rows)
                {
                    int remaining = Convert.ToInt32(m["Qty"]);

                    if (remaining > 0)
                    {
                        decimal price = Convert.ToDecimal(m["Price"]);

                        DataRow row = cart.NewRow();
                        row["ItemId"] = m["POI_ItemId"].ToString();
                        row["ItemName"] = m["Item_Name"].ToString();
                        row["Qty"] = remaining;
                        row["Price"] = price.ToString("0.00");
                        row["Amount"] = (remaining * price).ToString("0.00");
                        cart.Rows.Add(row);
                    }
                }
            }

            ViewState["BOItems"] = cart;

            bindItemsGrid();
        }


        protected void drppo_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillItemCombo();
            fillPrice();
            fillPending();
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



        // Save Back Order (header + item lines)
        protected void add_bo_btn_Click(object sender, EventArgs e)
        {
            if (drppo.Items.Count == 0)
            {
                Response.Write("<script>alert('There is no Purchase Order with missing items')</script>");
                return;
            }

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

            // Insert Back Order header
            cmd = new SqlCommand(
                "INSERT INTO BackOrder_Tbl (BO_PO_Id, BO_Remarks, BO_TotalAmount, BO_Status) " +
                "VALUES ('" + drppo.SelectedValue + "', '" + txtremarks.Text + "', '" + total + "', 'Pending'); " +
                "SELECT SCOPE_IDENTITY();",
                con
            );

            int newBoId = Convert.ToInt32(cmd.ExecuteScalar());

            // Insert each item line
            foreach (DataRow r in dt.Rows)
            {
                cmd = new SqlCommand(
                    "INSERT INTO BO_Items_Tbl (BOI_BO_Id, BOI_ItemId, BOI_Qty, BOI_Price, BOI_Amount) " +
                    "VALUES ('" + newBoId + "', '" + r["ItemId"] + "', '" + r["Qty"] + "', '" + r["Price"] + "', '" + r["Amount"] + "')",
                    con
                );
                cmd.ExecuteNonQuery();
            }

            ViewState["BOItems"] = null;

            Response.Write("<script>alert('Back Order Saved Successfully')</script>");
            Response.Redirect("BackOrder.aspx");
        }


        protected void LinkButton1_Click(object sender, EventArgs e)
        {
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
                fillPOCombo();
                fillItemCombo();
                fillPrice();

                lblWelcome.Text = "Welcome, " + Session["admin"].ToString();

                fillPending();
            }
        }
    }
}
