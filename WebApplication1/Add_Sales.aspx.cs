using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Add_Sales : System.Web.UI.Page
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


        // A new sale has no stock rows of its own yet
        string getOwnRef()
        {
            return "";
        }


        // Fill Item Dropdown (all active items, shown as "Item - Supplier")
        void fillItemCombo()
        {
            getCon();

            da = new SqlDataAdapter(
                "SELECT i.Item_Id, i.Item_Name + ' - ' + s.Supp_Name AS ItemDisplay " +
                "FROM Item_Tbl i INNER JOIN Supplier_Tbl s ON i.Item_SupplierId = s.Supp_Id " +
                "WHERE i.Item_Status = 1 ORDER BY i.Item_Name ASC",
                con
            );

            ds = new DataSet();
            da.Fill(ds);

            drpitem.DataSource = ds.Tables[0];
            drpitem.DataTextField = "ItemDisplay";
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
            dt.Columns.Add("Cost");
            return dt;
        }


        // Get the items cart from ViewState
        DataTable getItemsTable()
        {
            if (ViewState["SalItems"] == null)
            {
                ViewState["SalItems"] = newItemsTable();
            }
            return (DataTable)ViewState["SalItems"];
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


        // Available stock of one item (IN - OUT), without the stock rows of this sale itself
        int getAvailable(string itemId)
        {
            getCon();

            string sql =
                "select isnull(sum(case when Stock_Type = 'IN' then Stock_Qty when Stock_Type = 'OUT' then -Stock_Qty else 0 end), 0) " +
                "from Stock_Tbl where Stock_ItemId = '" + itemId + "'";

            if (getOwnRef() != "")
            {
                sql += " and (Stock_Ref is null or Stock_Ref <> '" + getOwnRef() + "')";
            }

            cmd = new SqlCommand(sql, con);

            int available = Convert.ToInt32(cmd.ExecuteScalar());

            con.Close();

            return available;
        }


        // Selling price of the selected item + its available stock
        void fillPrice()
        {
            if (drpitem.Items.Count == 0)
            {
                txtprice.Text = "";
                lblavail.Text = "0";
                return;
            }

            getCon();

            cmd = new SqlCommand("select Item_SellingPrice from Item_Tbl where Item_Id = '" + drpitem.SelectedValue + "'", con);

            txtprice.Text = Convert.ToDecimal(cmd.ExecuteScalar()).ToString("0.00");

            con.Close();

            lblavail.Text = getAvailable(drpitem.SelectedValue).ToString();
        }


        protected void drpitem_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillPrice();
        }


        // Returns "" when stock is enough for every line, otherwise the message to show
        string checkStock(DataTable dt)
        {
            foreach (DataRow r in dt.Rows)
            {
                int need = 0;

                foreach (DataRow x in dt.Rows)
                {
                    if (x["ItemId"].ToString() == r["ItemId"].ToString())
                    {
                        need += Convert.ToInt32(x["Qty"]);
                    }
                }

                int available = getAvailable(r["ItemId"].ToString());

                if (need > available)
                {
                    return "Not enough stock for " + r["ItemName"].ToString().Replace("'", "") + " (available " + available + ")";
                }
            }

            return "";
        }


        // Cost price of an item (saved with the sale line, used by Profit / Loss)
        decimal getItemCost(string itemId)
        {
            getCon();

            cmd = new SqlCommand("select Item_Cost from Item_Tbl where Item_Id = '" + itemId + "'", con);

            decimal cost = Convert.ToDecimal(cmd.ExecuteScalar());

            con.Close();

            return cost;
        }


        // Add current item selection into the cart
        protected void btn_additem_Click(object sender, EventArgs e)
        {
            if (drpitem.Items.Count == 0)
            {
                Response.Write("<script>alert('There are no items to sell')</script>");
                return;
            }

            int qty;
            decimal price;

            if (!int.TryParse(txtqty.Text, out qty) || qty <= 0 || !decimal.TryParse(txtprice.Text, out price))
            {
                Response.Write("<script>alert('Please enter a valid Qty and Price')</script>");
                return;
            }

            decimal amount = qty * price;

            DataTable dt = getItemsTable();

            DataRow row = dt.NewRow();
            row["ItemId"] = drpitem.SelectedValue;
            row["ItemName"] = drpitem.SelectedItem.Text;
            row["Qty"] = qty;
            row["Price"] = price.ToString("0.00");
            row["Amount"] = amount.ToString("0.00");
            row["Cost"] = getItemCost(drpitem.SelectedValue).ToString("0.00");

            dt.Rows.Add(row);

            // cannot sell more than what is in stock
            string msg = checkStock(dt);

            if (msg != "")
            {
                dt.Rows.Remove(row);
                Response.Write("<script>alert('" + msg + "')</script>");
                return;
            }

            ViewState["SalItems"] = dt;

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

                ViewState["SalItems"] = dt;

                bindItemsGrid();
            }
        }



        // Save Sale (header + item lines), then take the items out of stock
        protected void add_sale_btn_Click(object sender, EventArgs e)
        {
            if (txtclient.Text.Trim() == "")
            {
                Response.Write("<script>alert('Please enter the client name')</script>");
                return;
            }

            DataTable dt = getItemsTable();

            if (dt.Rows.Count == 0)
            {
                Response.Write("<script>alert('Please add at least one item to sell')</script>");
                return;
            }

            string msg = checkStock(dt);

            if (msg != "")
            {
                Response.Write("<script>alert('" + msg + "')</script>");
                return;
            }

            decimal total = 0;
            foreach (DataRow r in dt.Rows)
            {
                total += Convert.ToDecimal(r["Amount"]);
            }

            getCon();

            // Insert Sale header
            cmd = new SqlCommand(
                "INSERT INTO Sales_Tbl (SAL_Client, SAL_Remarks, SAL_TotalAmount) " +
                "VALUES ('" + txtclient.Text + "', '" + txtremarks.Text + "', '" + total + "'); " +
                "SELECT SCOPE_IDENTITY();",
                con
            );

            int newSalId = Convert.ToInt32(cmd.ExecuteScalar());

            // Insert each item line + stock out
            foreach (DataRow r in dt.Rows)
            {
                cmd = new SqlCommand(
                    "INSERT INTO Sales_Items_Tbl (SALI_SAL_Id, SALI_ItemId, SALI_Qty, SALI_Price, SALI_Cost, SALI_Amount) " +
                    "VALUES ('" + newSalId + "', '" + r["ItemId"] + "', '" + r["Qty"] + "', '" + r["Price"] + "', '" + r["Cost"] + "', '" + r["Amount"] + "')",
                    con
                );
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand(
                    "INSERT INTO Stock_Tbl (Stock_ItemId, Stock_Qty, Stock_Type, Stock_Ref) " +
                    "VALUES ('" + r["ItemId"] + "', '" + r["Qty"] + "', 'OUT', 'SAL-" + newSalId + "')",
                    con
                );
                cmd.ExecuteNonQuery();
            }

            ViewState["SalItems"] = null;

            Response.Write("<script>alert('Sale Saved Successfully')</script>");
            Response.Redirect("Sales.aspx");
        }


        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Sales.aspx");
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

                lblWelcome.Text = "Welcome, " + Session["admin"].ToString();

                ViewState["SalItems"] = newItemsTable();
                bindItemsGrid();
            }
        }
    }
}
