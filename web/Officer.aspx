<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Officer.aspx.cs" Inherits="web.Officer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <br />
        <asp:Button ID="btnChangePw" runat="server" OnClick="btnChangePw_Click" Text="變更密碼" />

    </p>

    <asp:DropDownList ID="ddlRoom" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRoom_SelectedIndexChanged">
        <asp:ListItem Text="--請選擇教室--" Value="0"></asp:ListItem>
    </asp:DropDownList>

    <asp:Button ID="btnPrevWeek" runat="server" Text="上一週" OnClick="btnPrevWeek_Click" />
    <asp:Button ID="btnNextWeek" runat="server" Text="下一週" OnClick="btnNextWeek_Click" />
    
    <div class="grid-container">
        <div class="grid">
            <asp:GridView ID="gvSchedule" runat="server" AutoGenerateColumns="False" CssClass="gridview-table">
                <Columns>
                    <asp:BoundField DataField="Period" HeaderText="節次" />
                    <asp:TemplateField HeaderText="星期一">
                        <ItemTemplate>
                            <%# Eval("Day1") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="星期二">
                        <ItemTemplate>
                            <%# Eval("Day2") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="星期三">
                        <ItemTemplate>
                            <%# Eval("Day3") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="星期四">
                        <ItemTemplate>
                            <%# Eval("Day4") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="星期五">
                        <ItemTemplate>
                            <%# Eval("Day5") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

        <div class="grid">
            <br />
            <asp:GridView ID="gvEquip" runat="server" AutoGenerateColumns="False" CssClass="gridview-table">
                <Columns>
                    <asp:BoundField DataField="equipid" HeaderText="設備編號" />
                    <asp:BoundField DataField="equipname" HeaderText="設備名稱" />
                    <asp:TemplateField HeaderText="已借出">
                        <ItemTemplate>
                            <%# Convert.ToBoolean(Eval("borrowed")) ? "是" : "否" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    
    <br />
    <asp:Button ID="btnConfirmReturn" runat="server" Text="確認歸還" OnClick="btnConfirmReturn_Click" />
    <asp:Label ID="Label2" runat="server" Text="沒有申請單資料可以顯示" Visible="False"></asp:Label>
    <div>
            <asp:GridView ID="gvApply" runat="server" AutoGenerateColumns="False" CssClass="gridview-table" DataKeyNames="applyid">
                <Columns>
                    <asp:BoundField DataField="applyid" HeaderText="申請編號" />
                    <asp:BoundField DataField="applier" HeaderText="申請者" />
                    <asp:BoundField DataField="roomid" HeaderText="教室" />
                    <asp:BoundField DataField="using_hour" HeaderText="借用節次" />
                    <asp:BoundField DataField="using_date" HeaderText="借用日期" />
                    <asp:BoundField DataField="purpose" HeaderText="用途" />
                    <asp:TemplateField HeaderText="借用設備">
                        <ItemTemplate>
                            <asp:Literal ID="litEquipNames" runat="server" Text='<%# Eval("EquipNames") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="officeid" HeaderText="負責系辦" />
                    <asp:BoundField DataField="duedate" HeaderText="歸還日期" />
                    <asp:TemplateField HeaderText="已歸還">
                        <ItemTemplate>
                            <asp:Literal ID="litReturned" runat="server" 
                                Visible='<%# Convert.ToBoolean(Eval("returned")) %>' 
                                Text="☑"></asp:Literal>
                            <asp:CheckBox ID="cbReturn" runat="server" 
                                Visible='<%# !Convert.ToBoolean(Eval("returned")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>


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
