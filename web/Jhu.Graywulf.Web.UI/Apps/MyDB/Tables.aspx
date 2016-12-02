<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Tables" CodeBehind="Tables.aspx.cs" %>

<%@ Register Src="Toolbar.ascx" TagPrefix="jgwc" TagName="Toolbar" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <jgwc:Toolbar runat="server" id="toolbar" SelectedTab="tables"
        OnSelectedDatasetChanged="Toolbar_SelectedDatasetChanged" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <div class="dock-bottom">
        <p class="FormMessage">
            <asp:CustomValidator ID="TableSelectedValidator" runat="server" ErrorMessage="No table was selected."
                OnServerValidate="TableSelected_ServerValidate" ValidationGroup="Table" />
            <asp:CustomValidator ID="SingleTableSelectedValidator" runat="server" ErrorMessage="No table was selected."
                OnServerValidate="SingleTableSelectedValidator_ServerValidate" ValidationGroup="SingleTable" /></p>
        <p class="FormButtons">
            <asp:Button ID="view" runat="server" Text="View Schema" CssClass="FormButton" CommandName="View"
                OnCommand="Button_Command" ValidationGroup="SingleTable" />
            <asp:Button ID="Edit" runat="server" Text="Edit Schema" CssClass="FormButton" CommandName="Edit" Visible="false" ValidationGroup="SingleTable" />
            |
            <asp:Button ID="peek" runat="server" Text="Peek" CssClass="FormButton" CommandName="Peek"
                OnCommand="Button_Command" ValidationGroup="SingleTable" />
            <asp:Button ID="export" runat="server" Text="Export" CssClass="FormButton" CommandName="Export"
                OnCommand="Button_Command" ValidationGroup="SingleTable" />
            <asp:Button ID="rename" runat="server" Text="Rename" CssClass="FormButton" CommandName="Rename"
                OnCommand="Button_Command" ValidationGroup="SingleTable" />
            <asp:Button ID="primaryKey" runat="server" Text="Primary Key" CssClass="FormButton" CommandName="PrimaryKey"
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
                <asp:BoundField DataField="Metadata.DateCreated" HeaderText="Created" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="Metadata.DateModified" HeaderText="Modified" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center"/>
                <asp:BoundField DataField="" HeaderText="Comments" ItemStyle-CssClass="GridViewSpan"
                    HeaderStyle-CssClass="GridViewSpan" />
            </Columns>
        </jgwc:MultiSelectGridView>
    </div>
</asp:Content>
