<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditProfile.aspx.cs" Inherits="web.EditProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <br />
        <asp:Button ID="ButtonGoback" runat="server" OnClick="ButtonGoback_Click" Text="回到教室借用" />
    </p>
    <p>
        &nbsp;</p>
    <p>
&nbsp;&nbsp; 帳號(學號)&nbsp;&nbsp;&nbsp;
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    </p>
    <p>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 密碼&nbsp;&nbsp;&nbsp; 
        <asp:TextBox ID="TextBoxPw" runat="server" OnTextChanged="TextBoxPassword_TextChanged"></asp:TextBox>
    </p>
    <p>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 暱稱&nbsp;&nbsp;&nbsp; 
        <asp:TextBox ID="TextBoxName" runat="server" OnTextChanged="TextBoxName_TextChanged"></asp:TextBox>
    </p>
    <p>
&nbsp;&nbsp; 實驗室編號&nbsp;&nbsp;&nbsp; 
        <asp:TextBox ID="TextBoxOffice" runat="server" OnTextChanged="TextBoxOffice_TextChanged"></asp:TextBox>
    </p>
    <p>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 連絡電話&nbsp;&nbsp;&nbsp; 
        <asp:TextBox ID="TextBoxPhone" runat="server" OnTextChanged="TextBoxPhone_TextChanged"></asp:TextBox>
    </p>
    <p>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 電子信箱&nbsp;&nbsp;&nbsp; 
        <asp:TextBox ID="TextBoxEmail" runat="server" OnTextChanged="TextBoxEmail_TextChanged"></asp:TextBox>
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
