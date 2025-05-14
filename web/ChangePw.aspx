<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangePw.aspx.cs" Inherits="web.ChangePw" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <br />
        <asp:Button ID="ButtonGoback" runat="server" OnClick="ButtonGoback_Click" Text="回到申請表一覽" />
    </p>
    <p>
        &nbsp;</p>
    <p>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 帳號&nbsp;&nbsp;&nbsp;
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    </p>
    <p>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;舊密碼&nbsp;&nbsp;&nbsp; 
        <asp:TextBox ID="oldPw" runat="server" OnTextChanged="TextBoxPassword_TextChanged"></asp:TextBox>
    </p>
    <p>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;新密碼&nbsp;&nbsp;&nbsp; 
        <asp:TextBox ID="newPw" runat="server" OnTextChanged="TextBoxPassword_TextChanged"></asp:TextBox>
    </p>
    <p>
        &nbsp;&nbsp;確認新密碼&nbsp;&nbsp;&nbsp; 
        <asp:TextBox ID="checkPw" runat="server" OnTextChanged="TextBoxPassword_TextChanged"></asp:TextBox>
    </p>
    <p>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="ButtonUpdate" runat="server" OnClick="ButtonUpdate_Click" Text="確認修改" />
&nbsp;&nbsp;&nbsp;
        <asp:Label ID="Label2" runat="server" Text="修改成功" Visible="False"></asp:Label>
    </p>
    <p>
        &nbsp;</p>
</asp:Content>