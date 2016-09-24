<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Tables" CodeBehind="Tables.aspx.cs" %>

<%@ Register Src="~/MyDb/Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <div class="dock-top">
        <jgwc:MyDbTabs ID="MyDbTabs1" runat="server" SelectedTab="Tables" />
    </div>
    <div class="dock-bottom">
        <p class="FormMessage">
            <asp:CustomValidator ID="TableSelectedValidator" runat="server" ErrorMessage="No table was selected."
                OnServerValidate="TableSelected_ServerValidate" ValidationGroup="Table" />
            <asp:CustomValidator ID="SingleTableSelectedValidator" runat="server" ErrorMessage="No table was selected."
                OnServerValidate="SingleTableSelectedValidator_ServerValidate" ValidationGroup="SingleTable" /></p>
        <p class="FormButtons">
            <asp:Button ID="View" runat="server" Text="View Schema" CssClass="FormButton" CommandName="View"
                OnCommand="Button_Command" ValidationGroup="SingleTable" />
            <asp:Button ID="Edit" runat="server" Text="Edit Schema" CssClass="FormButton" CommandName="Edit" Visible="false" ValidationGroup="SingleTable" />
            |
            <asp:Button ID="Peek" runat="server" Text="Peek" CssClass="FormButton" CommandName="Peek"
                OnCommand="Button_Command" ValidationGroup="SingleTable" />
            <asp:Button ID="Export" runat="server" Text="Export" CssClass="FormButton" CommandName="Export"
                OnCommand="Button_Command" ValidationGroup="SingleTable" />
            <asp:Button ID="Rename" runat="server" Text="Rename" CssClass="FormButton" CommandName="Rename"
                OnCommand="Button_Command" ValidationGroup="SingleTable" />
            |
            <asp:Button ID="Drop" runat="server" Text="Drop" CssClass="FormButton" CommandName="Drop"
                OnCommand="Button_Command" ValidationGroup="Table" />
        </p>
    </div>
    <div class="TabFrame dock-fill dock-scroll">
        <jgwc:MultiSelectGridView runat="server" ID="TableList" AllowPaging="true" PageSize="25" AutoGenerateColumns="false"
            SelectionMode="Multiple" DataKeyNames="UniqueKey" OnPageIndexChanging="TableList_PageIndexChanging">
            <Columns>
                <jgwc:SelectionField ItemStyle-Width="24px" />
                <asp:TemplateField HeaderText="Name" ItemStyle-Width="300px" ItemStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <%# Eval("DatasetName") %>:<%# Eval("SchemaName") %>.<b><%# Eval("TableName") %></b>
                    </ItemTemplate>
                </asp:TemplateField>
                <jgwc:BoundField DataField="Statistics.RowCount" HeaderText="Rows" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
                <jgwc:BoundByteSizeField DataField="Statistics.DataSpace" HeaderText="Data Size" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
                <jgwc:BoundByteSizeField DataField="Statistics.IndexSpace" HeaderText="Index Size" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="" HeaderText="Comments" ItemStyle-CssClass="GridViewSpan"
                    HeaderStyle-CssClass="GridViewSpan" />
            </Columns>
        </jgwc:MultiSelectGridView>
    </div>
</asp:Content>
