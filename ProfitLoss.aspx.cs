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
    public partial class ProfitLoss : System.Web.UI.Page
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


        // Show one box: money collected, cost and profit / loss from a start date until now
        void showBox(DataTable rows, DateTime from, Label lblCollected, Label lblCost, Label lblPL, Label lblProfit)
        {
            decimal collected = 0;
            decimal cost = 0;
            decimal profit = 0;

            foreach (DataRow r in rows.Rows)
            {
                if (Convert.ToDateTime(r["SAL_DateCreated"]) >= from)
                {
                    collected += Convert.ToDecimal(r["SalesAmt"]);
                    cost += Convert.ToDecimal(r["CostAmt"]);
                    profit += Convert.ToDecimal(r["Profit"]);
                }
            }

            lblCollected.Text = collected.ToString("N2");
            lblCost.Text = cost.ToString("N2");

            if (profit >= 0)
            {
                lblPL.Text = "Profit";
                lblPL.CssClass = "badge badge-success";
                lblProfit.CssClass = "font-weight-bold text-success";
                lblProfit.Text = profit.ToString("N2");
            }
            else
            {
                lblPL.Text = "Loss";
                lblPL.CssClass = "badge badge-danger";
                lblProfit.CssClass = "font-weight-bold text-danger";
                lblProfit.Text = Math.Abs(profit).ToString("N2");
            }
        }


        // Group the sale lines into periods (newest first) - only periods with sales - and add a Total row at the end
        DataTable buildTable(DataTable rows, List<string> labels, List<DateTime> starts, List<DateTime> ends)
        {
            DataTable t = new DataTable();
            t.Columns.Add("Period", typeof(string));
            t.Columns.Add("Qty", typeof(int));
            t.Columns.Add("Sales", typeof(decimal));
            t.Columns.Add("Cost", typeof(decimal));
            t.Columns.Add("Profit", typeof(decimal));

            int totalQty = 0;
            decimal totalSales = 0;
            decimal totalCost = 0;
            decimal totalProfit = 0;

            // newest period first
            for (int i = starts.Count - 1; i >= 0; i--)
            {
                int qty = 0;
                decimal sales = 0;
                decimal cost = 0;
                decimal profit = 0;
                bool hasSale = false;

                foreach (DataRow r in rows.Rows)
                {
                    DateTime saleDate = Convert.ToDateTime(r["SAL_DateCreated"]);

                    if (saleDate >= starts[i] && saleDate < ends[i])
                    {
                        hasSale = true;
                        qty += Convert.ToInt32(r["SALI_Qty"]);
                        sales += Convert.ToDecimal(r["SalesAmt"]);
                        cost += Convert.ToDecimal(r["CostAmt"]);
                        profit += Convert.ToDecimal(r["Profit"]);
                    }
                }

                // periods without any sale are not shown
                if (hasSale)
                {
                    t.Rows.Add(labels[i], qty, sales, cost, profit);

                    totalQty += qty;
                    totalSales += sales;
                    totalCost += cost;
                    totalProfit += profit;
                }
            }

            // Total row only when there is at least one period with sales
            if (t.Rows.Count > 0)
            {
                t.Rows.Add("Total", totalQty, totalSales, totalCost, totalProfit);
            }

            return t;
        }


        void loadData()
        {
            DateTime today = DateTime.Today;
            DateTime weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));

            // all sale lines of the last 5 years (every table below is inside this range)
            getCon();

            da = new SqlDataAdapter(
                "select sa.SAL_DateCreated, si.SALI_Qty, si.SALI_Amount as SalesAmt, si.SALI_Cost * si.SALI_Qty as CostAmt, " +
                "(si.SALI_Price - si.SALI_Cost) * si.SALI_Qty as Profit " +
                "from Sales_Items_Tbl si inner join Sales_Tbl sa on si.SALI_SAL_Id = sa.SAL_Id " +
                "where sa.SAL_DateCreated >= '" + new DateTime(today.Year - 4, 1, 1).ToString("yyyyMMdd") + "'",
                con
            );
            ds = new DataSet();
            da.Fill(ds);

            con.Close();

            DataTable rows = ds.Tables[0];

            // ---------- the four boxes ----------
            showBox(rows, today, lblDayCollected, lblDayCost, lblDayPL, lblDayProfit);
            showBox(rows, weekStart, lblWeekCollected, lblWeekCost, lblWeekPL, lblWeekProfit);
            showBox(rows, new DateTime(today.Year, today.Month, 1), lblMonthCollected, lblMonthCost, lblMonthPL, lblMonthProfit);
            showBox(rows, new DateTime(today.Year, 1, 1), lblYearCollected, lblYearCost, lblYearPL, lblYearProfit);

            // ---------- daily: last 30 days ----------
            List<string> labels = new List<string>();
            List<DateTime> starts = new List<DateTime>();
            List<DateTime> ends = new List<DateTime>();

            for (int i = 29; i >= 0; i--)
            {
                DateTime d = today.AddDays(-i);
                starts.Add(d);
                ends.Add(d.AddDays(1));
                labels.Add(d.ToString("dd MMM yyyy (ddd)"));
            }

            GridView1.DataSource = buildTable(rows, labels, starts, ends);
            GridView1.DataBind();

            // ---------- weekly: last 12 weeks ----------
            labels = new List<string>();
            starts = new List<DateTime>();
            ends = new List<DateTime>();

            for (int i = 11; i >= 0; i--)
            {
                DateTime d = weekStart.AddDays(-7 * i);
                starts.Add(d);
                ends.Add(d.AddDays(7));
                labels.Add(d.ToString("dd MMM") + " - " + d.AddDays(6).ToString("dd MMM yyyy"));
            }

            GridView2.DataSource = buildTable(rows, labels, starts, ends);
            GridView2.DataBind();

            // ---------- monthly: last 12 months ----------
            labels = new List<string>();
            starts = new List<DateTime>();
            ends = new List<DateTime>();

            for (int i = 11; i >= 0; i--)
            {
                DateTime d = new DateTime(today.Year, today.Month, 1).AddMonths(-i);
                starts.Add(d);
                ends.Add(d.AddMonths(1));
                labels.Add(d.ToString("MMMM yyyy"));
            }

            GridView3.DataSource = buildTable(rows, labels, starts, ends);
            GridView3.DataBind();

            // ---------- yearly: last 5 years ----------
            labels = new List<string>();
            starts = new List<DateTime>();
            ends = new List<DateTime>();

            for (int i = 4; i >= 0; i--)
            {
                DateTime d = new DateTime(today.Year - i, 1, 1);
                starts.Add(d);
                ends.Add(d.AddYears(1));
                labels.Add(d.ToString("yyyy"));
            }

            GridView4.DataSource = buildTable(rows, labels, starts, ends);
            GridView4.DataBind();
        }


        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Session["admin"] = null;
            Response.Redirect("login.aspx");
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["admin"] == null)
            {
                Response.Redirect("login.aspx");
            }

            if (!IsPostBack)
            {
                lblDate.Text = DateTime.Today.ToString("dddd, dd MMMM yyyy");

                loadData();
            }
        }
    }
}
