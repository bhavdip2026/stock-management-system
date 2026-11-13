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
    public partial class Edt_Supplier : System.Web.UI.Page
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

        void fillData()
        {
            getCon();
            da = new SqlDataAdapter("select * from Supplier_Tbl where Supp_Id ='" + Request.QueryString["id"] + "'", con);
            ds = new DataSet();
            da.Fill(ds);

            txtnm.Text = ds.Tables[0].Rows[0]["Supp_Name"].ToString();
            txtadd.Text = ds.Tables[0].Rows[0]["Supp_Address"].ToString();
            txtcp.Text = ds.Tables[0].Rows[0]["Supp_CPerson"].ToString();
            txtcontact.Text = ds.Tables[0].Rows[0]["Supp_Contact"].ToString();
            drpstatus.SelectedValue = ds.Tables[0].Rows[0]["Supp_Status"].ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null)
            {
                Response.Redirect("login.aspx");
            }

            if (!IsPostBack)
            {
                fillData();
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Supplier.aspx");
        }

        protected void edt_sup_btn_Click(object sender, EventArgs e)
        {
            getCon();

            cmd = new SqlCommand("SELECT COUNT(*) FROM Supplier_Tbl WHERE Supp_Name='" + txtnm.Text + "' AND Supp_Id != '" + Request.QueryString["id"] + "'", con);
            int i = Convert.ToInt32(cmd.ExecuteScalar());

            if (i > 0)
            {
                Response.Write("<script>alert('Supplier Name already exist')</script>");
                return;
            }

            cmd = new SqlCommand("update Supplier_Tbl set Supp_Name='" + txtnm.Text + "', Supp_Address='" + txtadd.Text + "', Supp_CPerson='" + txtcp.Text + "', Supp_Contact='" + txtcontact.Text + "', Supp_Status='" + drpstatus.SelectedValue + "' where Supp_Id='" + Request.QueryString["id"] + "'", con);
            cmd.ExecuteNonQuery();

            Response.Write("<script>alert('Supplier Updated Successfully')</script>");
            Response.Redirect("Supplier.aspx");
        }
    }
}
