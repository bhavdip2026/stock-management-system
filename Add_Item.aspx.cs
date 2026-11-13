using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Add_Item : System.Web.UI.Page
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


        // Clear form
        void clear()
        {
            txtnm.Text = "";
            txtdesc.Text = "";
            txtcost.Text = "";
            txtsp.Text = "";

            drpsupplier.SelectedIndex = 0;
            drpstatus.SelectedIndex = 0;
        }


        // Fill Supplier Dropdown
        void fillCombo()
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


        // Add Item
        protected void add_item_btn_Click(object sender, EventArgs e)
        {
            getCon();

            // Check duplicate item under same supplier
            cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Item_Tbl WHERE Item_Name='" + txtnm.Text + "' AND Item_SupplierId='" + drpsupplier.SelectedValue + "'",
                con
            );

            int i = Convert.ToInt32(cmd.ExecuteScalar());

            if (i > 0)
            {
                Response.Write("<script>alert('Item already exists under selected supplier')</script>");
                return;
            }


            cmd = new SqlCommand(
                "INSERT INTO Item_Tbl (Item_Name, Item_Desc, Item_Cost, Item_SellingPrice, Item_SupplierId, Item_Status) " +
                "VALUES ('" + txtnm.Text + "', '" + txtdesc.Text + "', '" + txtcost.Text + "', '" + txtsp.Text + "', '" + drpsupplier.SelectedValue + "', '" + drpstatus.SelectedValue + "')",
                con
            );

            cmd.ExecuteNonQuery();

            clear();

            Response.Write("<script>alert('Item Added Successfully')</script>");
        }


        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Item.aspx");
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
                fillCombo();

                lblWelcome.Text = "Welcome, " + Session["admin"].ToString();
            }
        }
    }
}
