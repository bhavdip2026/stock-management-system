using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace WebApplication1
{
    public partial class Add_Supplier : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter da;
        DataSet ds;

        string s = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        void getCon()
        {
            con = new SqlConnection(s);
            con.Open();
        }

        void clear()
        {
            txtnm.Text = string.Empty;
            txtadd.Text = string.Empty;
            txtcp.Text = string.Empty;
            txtcontact.Text = string.Empty;
            drpstatus.SelectedIndex = 0;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null)
            {
                Response.Redirect("login.aspx");
            }

            if (!IsPostBack)
            {
                lblresult.Text = "Welcome, " + Session["admin"].ToString();
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Supplier.aspx");
        }

        protected void add_sup_btn_Click(object sender, EventArgs e)
        {
            getCon();

            cmd = new SqlCommand("SELECT COUNT(*) FROM Supplier_Tbl WHERE Supp_Name='" + txtnm.Text + "'", con);
            int i = Convert.ToInt32(cmd.ExecuteScalar());

            if (i > 0)
            {
                Response.Write("<script>alert('Supplier Name already exist')</script>");
                return;
            }

            cmd = new SqlCommand("insert into Supplier_Tbl(Supp_Name, Supp_Address, Supp_CPerson, Supp_Contact, Supp_Status) values('" + txtnm.Text + "','" + txtadd.Text + "','" + txtcp.Text + "','" + txtcontact.Text + "','" + drpstatus.SelectedValue + "')", con);
            cmd.ExecuteNonQuery();
            clear();
            Response.Write("<script>alert('Supplier Added Successfully')</script>");
        }
    }
}
