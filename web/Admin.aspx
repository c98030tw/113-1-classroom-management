<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="web.Admin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="Panel1" runat="server" Height="147px" Width="369px">
    <br />
    <br />
    刪除指定的使用者<br /> 
    <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" OnLoad="DropDownList1_Load" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
        <asp:ListItem Text="--請選擇項目--" Value=""/>
    </asp:DropDownList>
        &nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="刪除" />
        &nbsp;
        <asp:Label ID="DeleteTarget" runat="server" Text="-1" Visible="False"></asp:Label>
        <br />
        <asp:Label ID="Label1" runat="server" Text="Label" Visible="False"></asp:Label>
        <br />
</asp:Panel>
    <br />
    <asp:Label ID="Label2" runat="server" Text="沒有資料可以顯示" Visible="False"></asp:Label>
<asp:GridView ID="GridView1" runat="server" 
    CssClass="table" GridLines="None" CellPadding="10" AutoGenerateEditButton="True" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowEditing="Gridview1_RowEditing" OnRowUpdating="GridView1_RowUpdating">
    
    <HeaderStyle Height="20px" />
    <PagerStyle VerticalAlign="Middle" />
    <RowStyle Height="20px" />
</asp:GridView>
</asp:Content>
