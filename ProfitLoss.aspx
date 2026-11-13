<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProfitLoss.aspx.cs" Inherits="WebApplication1.ProfitLoss" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Profit / Loss | Stock Management System</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />
    <link rel="stylesheet" href="css/adminlte.css" />
    <link rel="stylesheet" href="css/custom.css" />
</head>
<body class="hold-transition sidebar-mini layout-fixed">
    <form id="form1" runat="server">
        <div class="wrapper">

            <!-- Top Navbar -->
            <nav class="main-header navbar navbar-expand navbar-white navbar-light">
                <ul class="navbar-nav">
                    <li class="nav-item">
                        <a class="nav-link" data-widget="pushmenu" href="#" role="button"><i class="fas fa-bars"></i></a>
                    </li>
                    <li class="nav-item d-none d-sm-inline-block">
                        <span class="nav-link">Stock Management System</span>
                    </li>
                </ul>
                <ul class="navbar-nav ml-auto">
                    <li class="nav-item">
                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="nav-link" OnClick="LinkButton1_Click"><i class="fas fa-sign-out-alt"></i>&nbsp; Logout</asp:LinkButton>
                    </li>
                </ul>
            </nav>

            <!-- Sidebar -->
            <aside class="main-sidebar sidebar-dark-primary elevation-4">
                <a href="WebForm1.aspx" class="brand-link">
                    <img src="img/logo.png" alt="Logo" class="brand-image img-circle elevation-3" style="opacity: .8" />
                    <span class="brand-text font-weight-light">SMS Admin</span>
                </a>
                <div class="sidebar">
                    <nav class="mt-2">
                        <ul class="nav nav-pills nav-sidebar flex-column" data-widget="treeview" role="menu">
                            <li class="nav-item">
                                <a href="WebForm1.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-tachometer-alt"></i>
                                    <p>Dashboard</p>
                                </a>
                            </li>
                            <li class="nav-header">MAINTENANCE</li>
                            <li class="nav-item">
                                <a href="Supplier.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-truck"></i>
                                    <p>Supplier</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="Item.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-box"></i>
                                    <p>Item</p>
                                </a>
                            </li>
                            <li class="nav-header">INVENTORY</li>
                            <li class="nav-item">
                                <a href="Stocks.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-warehouse"></i>
                                    <p>Stocks</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="PurchaseOrder.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-file-invoice"></i>
                                    <p>Purchase Order</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="Receiving.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-dolly"></i>
                                    <p>Receiving</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="BackOrder.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-undo-alt"></i>
                                    <p>Back Order</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="Return.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-reply"></i>
                                    <p>Return</p>
                                </a>
                            </li>
                            <li class="nav-header">SALES</li>
                            <li class="nav-item">
                                <a href="Sales.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-cash-register"></i>
                                    <p>Sales</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="ProfitLoss.aspx" class="nav-link active">
                                    <i class="nav-icon fas fa-chart-line"></i>
                                    <p>Profit / Loss</p>
                                </a>
                            </li>
                            <li class="nav-header">SYSTEM</li>
                            <li class="nav-item">
                                <a href="Users.aspx" class="nav-link">
                                    <i class="nav-icon fas fa-users"></i>
                                    <p>Users</p>
                                </a>
                            </li>
                        </ul>
                    </nav>
                </div>
            </aside>

            <!-- Content Wrapper -->
            <div class="content-wrapper">
                <div class="content-header">
                    <div class="container-fluid">
                        <div class="row mb-2">
                            <div class="col-sm-8">
                                <h1 class="m-0">Profit / Loss</h1>
                                <span class="text-muted">How much money was collected from selling and how much was earned - as on
                                    <asp:Label ID="lblDate" runat="server"></asp:Label></span>
                            </div>
                        </div>
                    </div>
                </div>

                <section class="content">
                    <div class="container-fluid">

                        <div class="row">
                            <div class="col-lg-3 col-md-6">
                                <div class="card card-outline card-primary">
                                    <div class="card-body">
                                        <div class="text-muted text-uppercase small"><i class="fas fa-calendar-day"></i>&nbsp; Today</div>
                                        <div class="small text-muted mt-2">Money collected from selling</div>
                                        <h3 class="mb-2">
                                            <asp:Label ID="lblDayCollected" runat="server" Text="0.00"></asp:Label></h3>
                                        <div class="d-flex justify-content-between">
                                            <span class="text-muted">Cost of sold items</span>
                                            <b>
                                                <asp:Label ID="lblDayCost" runat="server" Text="0.00"></asp:Label></b>
                                        </div>
                                        <hr class="my-2" />
                                        <div class="d-flex justify-content-between align-items-center">
                                            <asp:Label ID="lblDayPL" runat="server" Text="Profit"></asp:Label>
                                            <span class="h5 mb-0">
                                                <asp:Label ID="lblDayProfit" runat="server" Text="0.00"></asp:Label></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-6">
                                <div class="card card-outline card-info">
                                    <div class="card-body">
                                        <div class="text-muted text-uppercase small"><i class="fas fa-calendar-week"></i>&nbsp; This Week</div>
                                        <div class="small text-muted mt-2">Money collected from selling</div>
                                        <h3 class="mb-2">
                                            <asp:Label ID="lblWeekCollected" runat="server" Text="0.00"></asp:Label></h3>
                                        <div class="d-flex justify-content-between">
                                            <span class="text-muted">Cost of sold items</span>
                                            <b>
                                                <asp:Label ID="lblWeekCost" runat="server" Text="0.00"></asp:Label></b>
                                        </div>
                                        <hr class="my-2" />
                                        <div class="d-flex justify-content-between align-items-center">
                                            <asp:Label ID="lblWeekPL" runat="server" Text="Profit"></asp:Label>
                                            <span class="h5 mb-0">
                                                <asp:Label ID="lblWeekProfit" runat="server" Text="0.00"></asp:Label></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-6">
                                <div class="card card-outline card-success">
                                    <div class="card-body">
                                        <div class="text-muted text-uppercase small"><i class="fas fa-calendar-alt"></i>&nbsp; This Month</div>
                                        <div class="small text-muted mt-2">Money collected from selling</div>
                                        <h3 class="mb-2">
                                            <asp:Label ID="lblMonthCollected" runat="server" Text="0.00"></asp:Label></h3>
                                        <div class="d-flex justify-content-between">
                                            <span class="text-muted">Cost of sold items</span>
                                            <b>
                                                <asp:Label ID="lblMonthCost" runat="server" Text="0.00"></asp:Label></b>
                                        </div>
                                        <hr class="my-2" />
                                        <div class="d-flex justify-content-between align-items-center">
                                            <asp:Label ID="lblMonthPL" runat="server" Text="Profit"></asp:Label>
                                            <span class="h5 mb-0">
                                                <asp:Label ID="lblMonthProfit" runat="server" Text="0.00"></asp:Label></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3 col-md-6">
                                <div class="card card-outline card-warning">
                                    <div class="card-body">
                                        <div class="text-muted text-uppercase small"><i class="fas fa-calendar"></i>&nbsp; This Year</div>
                                        <div class="small text-muted mt-2">Money collected from selling</div>
                                        <h3 class="mb-2">
                                            <asp:Label ID="lblYearCollected" runat="server" Text="0.00"></asp:Label></h3>
                                        <div class="d-flex justify-content-between">
                                            <span class="text-muted">Cost of sold items</span>
                                            <b>
                                                <asp:Label ID="lblYearCost" runat="server" Text="0.00"></asp:Label></b>
                                        </div>
                                        <hr class="my-2" />
                                        <div class="d-flex justify-content-between align-items-center">
                                            <asp:Label ID="lblYearPL" runat="server" Text="Profit"></asp:Label>
                                            <span class="h5 mb-0">
                                                <asp:Label ID="lblYearProfit" runat="server" Text="0.00"></asp:Label></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card card-outline card-primary">
                            <div class="card-header">
                                <h3 class="card-title">Daily - last 30 days (days with sales)</h3>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                                        CssClass="table table-bordered table-striped table-sm" GridLines="None" HeaderStyle-CssClass="thead-light"
                                        EmptyDataText="No sales yet.">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Day">
                                                <ItemTemplate>
                                                    <asp:Label ID="D1" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Period") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Items Sold" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="D2" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Qty") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Money Collected (Sales)" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="D3" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Sales", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cost" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="D4" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Cost", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Profit / Loss" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="D5" runat="server" CssClass='<%# Convert.ToDecimal(Eval("Profit")) >= 0 ? "text-success font-weight-bold" : "text-danger font-weight-bold" %>' Text='<%# Eval("Profit", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                        <div class="card card-outline card-info">
                            <div class="card-header">
                                <h3 class="card-title">Weekly - last 12 weeks (weeks with sales, Monday to Sunday)</h3>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False"
                                        CssClass="table table-bordered table-striped table-sm" GridLines="None" HeaderStyle-CssClass="thead-light"
                                        EmptyDataText="No sales yet.">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Week">
                                                <ItemTemplate>
                                                    <asp:Label ID="W1" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Period") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Items Sold" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="W2" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Qty") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Money Collected (Sales)" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="W3" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Sales", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cost" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="W4" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Cost", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Profit / Loss" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="W5" runat="server" CssClass='<%# Convert.ToDecimal(Eval("Profit")) >= 0 ? "text-success font-weight-bold" : "text-danger font-weight-bold" %>' Text='<%# Eval("Profit", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                        <div class="card card-outline card-success">
                            <div class="card-header">
                                <h3 class="card-title">Monthly - last 12 months (months with sales)</h3>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False"
                                        CssClass="table table-bordered table-striped table-sm" GridLines="None" HeaderStyle-CssClass="thead-light"
                                        EmptyDataText="No sales yet.">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Month">
                                                <ItemTemplate>
                                                    <asp:Label ID="M1" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Period") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Items Sold" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="M2" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Qty") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Money Collected (Sales)" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="M3" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Sales", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cost" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="M4" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Cost", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Profit / Loss" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="M5" runat="server" CssClass='<%# Convert.ToDecimal(Eval("Profit")) >= 0 ? "text-success font-weight-bold" : "text-danger font-weight-bold" %>' Text='<%# Eval("Profit", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                        <div class="card card-outline card-warning">
                            <div class="card-header">
                                <h3 class="card-title">Yearly - last 5 years (years with sales)</h3>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView4" runat="server" AutoGenerateColumns="False"
                                        CssClass="table table-bordered table-striped table-sm" GridLines="None" HeaderStyle-CssClass="thead-light"
                                        EmptyDataText="No sales yet.">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Year">
                                                <ItemTemplate>
                                                    <asp:Label ID="Y1" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Period") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Items Sold" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="Y2" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Qty") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Money Collected (Sales)" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="Y3" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Sales", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cost" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="Y4" runat="server" CssClass='<%# Eval("Period").ToString() == "Total" ? "font-weight-bold" : "" %>' Text='<%# Eval("Cost", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Profit / Loss" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-right">
                                                <ItemTemplate>
                                                    <asp:Label ID="Y5" runat="server" CssClass='<%# Convert.ToDecimal(Eval("Profit")) >= 0 ? "text-success font-weight-bold" : "text-danger font-weight-bold" %>' Text='<%# Eval("Profit", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>

                        <p class="text-muted small">
                            Money collected = total selling amount of the sales. Cost = cost price of the items that were sold (saved at the time of the sale).
                            Profit / Loss = money collected - cost. A red negative number means a loss. Purchase orders and returns are not part of this calculation.
                       
                        </p>

                    </div>
                </section>
            </div>

            <footer class="main-footer">
                <strong>Stock Management System</strong>
            </footer>
        </div>
    </form>
</body>
</html>
