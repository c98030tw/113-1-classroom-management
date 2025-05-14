<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="web.User" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <br />
        <asp:Button ID="ButtonProfile" runat="server" OnClick="ButtonProfile_Click" Text="編輯個人資料" />
    </p>

    <asp:DropDownList ID="ddlRooms" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRooms_SelectedIndexChanged">
        <asp:ListItem Text="--請選擇教室--" Value="0"></asp:ListItem>
    </asp:DropDownList>

    <asp:Button ID="btnPrevWeek" runat="server" Text="上一週" OnClick="btnPrevWeek_Click" />
    <asp:Button ID="btnNextWeek" runat="server" Text="下一週" OnClick="btnNextWeek_Click" />

    <asp:GridView ID="gvSchedule" runat="server" AutoGenerateColumns="False" CssClass="gridview-table"
        ShowHeaderWhenEmpty="True" OnRowDataBound="gvSchedule_RowDataBound">
    </asp:GridView>

    <asp:Label ID="lblSelected" runat="server" Text="已選節次：" />
    <br />
    <br />
    <asp:Button ID="btnConfirm" runat="server" Text="確認借用節次" OnClick="btnConfirm_Click" />
    &nbsp;&nbsp;
    <asp:Label ID="lblWarning" runat="server" Text="未勾選節次" ForeColor="Red" Visible="False" />


    <p>
        &nbsp;</p>
    <style>
        .gridview-table {
            border-collapse: collapse;
            width: 100%;
        }

        .gridview-table th, .gridview-table td {
            border: 1px solid #ddd;
            text-align: center; /* 水平居中 */
            vertical-align: middle; /* 垂直居中 */
            padding: 8px; /* 適當的內邊距 */
        }

        .gridview-table th {
            background-color: #f4f4f4; /* 表頭背景色 */
        }
    </style>

</asp:Content>
