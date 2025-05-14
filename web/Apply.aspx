<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Apply.aspx.cs" Inherits="web.Apply" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        .apply-table {
            border-collapse: collapse;
            width: 50%;
            margin: 20px auto;
        }

        .apply-table th, .apply-table td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }

        .apply-table th {
            background-color: #f4f4f4;
            text-align: center;
        }

        .action-container {
            text-align: center;
            margin-top: 20px;
        }
    </style>
    
    <asp:Table ID="tblApply" runat="server" CssClass="apply-table">
        <asp:TableRow>
            <asp:TableHeaderCell>申請資料</asp:TableHeaderCell>
            <asp:TableHeaderCell>內容</asp:TableHeaderCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>申請人</asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="lblApplicant" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>借用教室</asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="lblRoom" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>借用節數</asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="lblPeriod" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>借用日期</asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="lblDate" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>用途</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="txtPurpose" runat="server" Width="90%" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>負責系辦</asp:TableCell>
            <asp:TableCell>
                EC5011
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>借用設備</asp:TableCell>
            <asp:TableCell>
                <asp:PlaceHolder ID="phEquip" runat="server" EnableViewState="True" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    
    <div class="action-container">
        <asp:Button ID="btnSubmit" runat="server" Text="送出申請" OnClick="btnSubmit_Click" />
        <asp:Label ID="lblResult" runat="server" Visible="False" />
    </div>

</asp:Content>
