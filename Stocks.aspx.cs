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
    public partial class Stocks : System.Web.UI.Page
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

        // Stock per item: Available = all IN (receiving) - all OUT (sales)
        void fillGrid()
        {
            getCon();

            string search = txtsearch.Text.Replace("'", "''");

            da = new SqlDataAdapter(
                "select i.Item_Id, i.Item_Name, i.Item_Desc, s.Supp_Name, " +
                "max(case when st.Stock_Type = 'IN' then st.Stock_DateCreated end) as LastReceived, " +
                "isnull(sum(case when st.Stock_Type = 'IN' then st.Stock_Qty when st.Stock_Type = 'OUT' then -st.Stock_Qty else 0 end), 0) as Available " +
                "from Item_Tbl i " +
                "inner join Supplier_Tbl s on i.Item_SupplierId = s.Supp_Id " +
                "left join Stock_Tbl st on i.Item_Id = st.Stock_ItemId " +
                "where i.Item_Name like '%" + search + "%' or s.Supp_Name like '%" + search + "%' or i.Item_Desc like '%" + search + "%' " +
                "group by i.Item_Id, i.Item_Name, i.Item_Desc, s.Supp_Name " +
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

        protected void btn_search_Click(object sender, EventArgs e)
        {
            GridView1.PageIndex = 0;
            fillGrid();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            fillGrid();
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["admin"] = null;
            Response.Redirect("login.aspx");
        }
    }
}
