using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Edt_Receiving : System.Web.UI.Page
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


        // Purchase order info
        void fillData()
        {
            getCon();

            da = new SqlDataAdapter(
                "select p.PO_Status, p.PO_DateCreated, s.Supp_Name " +
                "from PO_Tbl p inner join Supplier_Tbl s on p.PO_SupplierId = s.Supp_Id " +
                "where p.PO_Id = '" + Request.QueryString["id"] + "' and p.PO_Status in ('Pending', 'Received')",
                con
            );
            ds = new DataSet();
            da.Fill(ds);

            con.Close();

            if (ds.Tables[0].Rows.Count == 0)
            {
                Response.Redirect("Receiving.aspx");
            }

            lblpo.Text = "PO #" + Request.QueryString["id"] + " - " + ds.Tables[0].Rows[0]["Supp_Name"].ToString();
            lbldate.Text = ds.Tables[0].Rows[0]["PO_DateCreated"].ToString();
            lblstatus.Text = ds.Tables[0].Rows[0]["PO_Status"].ToString();
        }


        // Item lines of the purchase order with the quantity to receive
        void fillGrid()
        {
            getCon();

            da = new SqlDataAdapter(
                "select poi.POI_Id, i.Item_Name, poi.POI_Qty, poi.POI_Price, isnull(poi.POI_RecvQty, poi.POI_Qty) as RecvQty " +
                "from PO_Items_Tbl poi inner join Item_Tbl i on poi.POI_ItemId = i.Item_Id " +
                "where poi.POI_PO_Id = '" + Request.QueryString["id"] + "' " +
                "order by poi.POI_Id",
                con
            );
            ds = new DataSet();
            da.Fill(ds);

            GridView2.DataSource = ds;
            GridView2.DataBind();

            con.Close();
        }


        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Receiving.aspx");
        }


        // Save the received quantities
        protected void edt_rec_btn_Click(object sender, EventArgs e)
        {
            // check every line first: 0 <= received <= ordered
            foreach (GridViewRow row in GridView2.Rows)
            {
                TextBox txtrecv = (TextBox)row.FindControl("txtrecv");
                Label lblordered = (Label)row.FindControl("lblordered");

                int received;
                int ordered = Convert.ToInt32(lblordered.Text);

                if (!int.TryParse(txtrecv.Text, out received) || received < 0)
                {
                    Response.Write("<script>alert('Row " + (row.RowIndex + 1) + ": please enter a valid received quantity')</script>");
                    return;
                }

                if (received > ordered)
                {
                    Response.Write("<script>alert('Row " + (row.RowIndex + 1) + ": received quantity cannot be more than the ordered quantity (" + ordered + ")')</script>");
                    return;
                }
            }

            getCon();

            // save the quantities
            foreach (GridViewRow row in GridView2.Rows)
            {
                TextBox txtrecv = (TextBox)row.FindControl("txtrecv");

                cmd = new SqlCommand(
                    "update PO_Items_Tbl set POI_RecvQty = '" + Convert.ToInt32(txtrecv.Text) + "' " +
                    "where POI_Id = '" + GridView2.DataKeys[row.RowIndex].Value + "'",
                    con
                );
                cmd.ExecuteNonQuery();
            }

            // if this PO was already received, the stock must follow the new quantities
            cmd = new SqlCommand("select PO_Status from PO_Tbl where PO_Id = '" + Request.QueryString["id"] + "'", con);

            if (cmd.ExecuteScalar().ToString() == "Received")
            {
                cmd = new SqlCommand("delete from Stock_Tbl where Stock_Ref = 'RCV-" + Request.QueryString["id"] + "'", con);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand(
                    "insert into Stock_Tbl (Stock_ItemId, Stock_Qty, Stock_Type, Stock_Ref) " +
                    "select POI_ItemId, POI_RecvQty, 'IN', 'RCV-" + Request.QueryString["id"] + "' " +
                    "from PO_Items_Tbl where POI_PO_Id = '" + Request.QueryString["id"] + "' and POI_RecvQty > 0",
                    con
                );
                cmd.ExecuteNonQuery();
            }

            Response.Write("<script>alert('Receiving Updated Successfully')</script>");
            Response.Redirect("Receiving.aspx");
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
                fillData();
                fillGrid();
            }
        }
    }
}
