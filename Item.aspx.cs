using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Item : System.Web.UI.Page
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

        void fillGrid()
        {
            getCon();
            da = new SqlDataAdapter(
                "select i.Item_Id, i.Item_Name, i.Item_Cost, i.Item_SellingPrice, i.Item_Status, s.Supp_Name " +
                "from Item_Tbl i inner join Supplier_Tbl s on i.Item_SupplierId = s.Supp_Id " +
                "order by i.Item_Name asc",
                con
            );
            ds = new DataSet();
            da.Fill(ds);
            GridView1.DataSource = ds;
            GridView1.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null)
            {
                Response.Redirect("login.aspx");
            }

            if (!IsPostBack)
            {
                fillGrid();
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "cmd_edt")
            {
                Response.Redirect("Edt_Item.aspx?id=" + e.CommandArgument);
            }
            else
            {
                getCon();
                cmd = new SqlCommand("delete from Item_Tbl where Item_Id = '" + e.CommandArgument + "'", con);
                cmd.ExecuteNonQuery();
                fillGrid();
                Response.Write("<script>alert('Item Deleted Successfully')</script>");
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["admin"] = null;
            Response.Redirect("login.aspx");
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Redirect("Add_Item.aspx");
        }
    }
}
