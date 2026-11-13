using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Edt_Item : System.Web.UI.Page
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


        // Fill Item Data
        void fillData()
        {
            getCon();

            da = new SqlDataAdapter(
                "select * from Item_Tbl where Item_Id ='" + Request.QueryString["id"] + "'",
                con
            );

            ds = new DataSet();

            da.Fill(ds);

            txtnm.Text = ds.Tables[0].Rows[0]["Item_Name"].ToString();
            txtdesc.Text = ds.Tables[0].Rows[0]["Item_Desc"].ToString();
            txtcost.Text = ds.Tables[0].Rows[0]["Item_Cost"].ToString();
            txtsp.Text = ds.Tables[0].Rows[0]["Item_SellingPrice"].ToString();

            drpsupplier.SelectedValue = ds.Tables[0].Rows[0]["Item_SupplierId"].ToString();
            drpstatus.SelectedValue = ds.Tables[0].Rows[0]["Item_Status"].ToString();
        }


        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Item.aspx");
        }


        // Update Item
        protected void edt_item_btn_Click(object sender, EventArgs e)
        {
            getCon();

            // Check duplicate item under same supplier, excluding itself
            cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Item_Tbl WHERE Item_Name='" + txtnm.Text + "' AND Item_SupplierId='" + drpsupplier.SelectedValue + "' AND Item_Id != '" + Request.QueryString["id"] + "'",
                con
            );

            int i = Convert.ToInt32(cmd.ExecuteScalar());

            if (i > 0)
            {
                Response.Write("<script>alert('Item already exists under selected supplier')</script>");
                return;
            }


            cmd = new SqlCommand(
                "update Item_Tbl set Item_Name='" + txtnm.Text + "', Item_Desc='" + txtdesc.Text + "', Item_Cost='" + txtcost.Text + "', Item_SellingPrice='" + txtsp.Text + "', Item_SupplierId='" + drpsupplier.SelectedValue + "', Item_Status='" + drpstatus.SelectedValue + "' where Item_Id='" + Request.QueryString["id"] + "'",
                con
            );

            cmd.ExecuteNonQuery();

            Response.Write("<script>alert('Item Updated Successfully')</script>");
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
                fillData();
            }
        }
    }
}
