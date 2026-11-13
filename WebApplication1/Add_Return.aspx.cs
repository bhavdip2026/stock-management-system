using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Add_Return : System.Web.UI.Page
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


        // A new return has no stock rows of its own yet
        string getOwnRef()
        {
            return "";
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


        // Fill Item Dropdown (only items of the selected supplier)
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


        protected void drpsupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillItemCombo();
            fillPrice();

            // items of another supplier cannot stay in this return
            ViewState["RetItems"] = newItemsTable();
            bindItemsGrid();
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
            if (ViewState["RetItems"] == null)
            {
                ViewState["RetItems"] = newItemsTable();
            }
            return (DataTable)ViewState["RetItems"];
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


        // Available stock of one item (IN - OUT), without the stock rows of this return itself
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


        // Cost price of the selected item + its available stock
        void fillPrice()
        {
            if (drpitem.Items.Count == 0)
            {
                txtprice.Text = "";
                lblavail.Text = "0";
                return;
            }

            getCon();

            cmd = new SqlCommand("select Item_Cost from Item_Tbl where Item_Id = '" + drpitem.SelectedValue + "'", con);

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


        // Add current item selection into the cart
        protected void btn_additem_Click(object sender, EventArgs e)
        {
            if (drpitem.Items.Count == 0)
            {
                Response.Write("<script>alert('There are no items for this supplier')</script>");
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

            dt.Rows.Add(row);

            // cannot return more than what is in stock
            string msg = checkStock(dt);

            if (msg != "")
            {
                dt.Rows.Remove(row);
                Response.Write("<script>alert('" + msg + "')</script>");
                return;
            }

            ViewState["RetItems"] = dt;

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

                ViewState["RetItems"] = dt;

                bindItemsGrid();
            }
        }



        // Save Return (header + item lines), then take the items out of stock
        protected void add_ret_btn_Click(object sender, EventArgs e)
        {
            if (drpsupplier.Items.Count == 0)
            {
                Response.Write("<script>alert('Please add a supplier first')</script>");
                return;
            }

            DataTable dt = getItemsTable();

            if (dt.Rows.Count == 0)
            {
                Response.Write("<script>alert('Please add at least one item to return')</script>");
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

            // Insert Return header
            cmd = new SqlCommand(
                "INSERT INTO Return_Tbl (RET_SupplierId, RET_Remarks, RET_TotalAmount) " +
                "VALUES ('" + drpsupplier.SelectedValue + "', '" + txtremarks.Text + "', '" + total + "'); " +
                "SELECT SCOPE_IDENTITY();",
                con
            );

            int newRetId = Convert.ToInt32(cmd.ExecuteScalar());

            // Insert each item line + stock out
            foreach (DataRow r in dt.Rows)
            {
                cmd = new SqlCommand(
                    "INSERT INTO Return_Items_Tbl (RETI_RET_Id, RETI_ItemId, RETI_Qty, RETI_Price, RETI_Amount) " +
                    "VALUES ('" + newRetId + "', '" + r["ItemId"] + "', '" + r["Qty"] + "', '" + r["Price"] + "', '" + r["Amount"] + "')",
                    con
                );
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand(
                    "INSERT INTO Stock_Tbl (Stock_ItemId, Stock_Qty, Stock_Type, Stock_Ref) " +
                    "VALUES ('" + r["ItemId"] + "', '" + r["Qty"] + "', 'OUT', 'RET-" + newRetId + "')",
                    con
                );
                cmd.ExecuteNonQuery();
            }

            ViewState["RetItems"] = null;

            Response.Write("<script>alert('Return Saved Successfully')</script>");
            Response.Redirect("Return.aspx");
        }


        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Return.aspx");
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
                fillSupplierCombo();
                fillItemCombo();
                fillPrice();

                lblWelcome.Text = "Welcome, " + Session["admin"].ToString();

                ViewState["RetItems"] = newItemsTable();
                bindItemsGrid();
            }
        }
    }
}
