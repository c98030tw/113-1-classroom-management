<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="web._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row" aria-expanded="undefined" aria-orientation="horizontal" aria-readonly="True" aria-required="False" aria-selected="undefined" aria-sort="other">
        <asp:Panel ID="Panel1" runat="server" Direction="LeftToRight" Height="250px" HorizontalAlign="Center" Width="400px">
            <br />
            <br />
            <asp:Label ID="Label1" runat="server" Text="帳號(學號或職位編號)"></asp:Label>
            &nbsp;
            <asp:TextBox ID="TextBox1" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
            <br />
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="Label2" runat="server" Text="密碼"></asp:Label>
            &nbsp;
            <asp:TextBox ID="TextBox2" runat="server" OnTextChanged="TextBox2_TextChanged"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="Label3" runat="server" ForeColor="Red" Text="Label" Visible="False"></asp:Label>
            <br />
            <br />
            <asp:Button ID="Button1" runat="server" Text="登入" OnClick="Button1_Click" />
            <br />
        </asp:Panel>
    </div>

</asp:Content>
