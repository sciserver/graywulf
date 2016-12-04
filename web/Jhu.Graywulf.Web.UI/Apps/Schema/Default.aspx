<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.Default" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="SummaryForm.ascx" TagName="SummaryForm" TagPrefix="list" %>
<%@ Register Src="ColumnList.ascx" TagName="ColumnList" TagPrefix="list" %>
<%@ Register Src="ParameterList.ascx" TagName="ParameterList" TagPrefix="list" %>
<%@ Register Src="IndexList.ascx" TagName="IndexList" TagPrefix="list" %>


<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <script>
        function ShowRefreshing() {
            var datasetList = document.getElementById("<%= DatasetList.ClientID %>");
            var objectTypeList = document.getElementById("<%= ObjectTypeList.ClientID %>");
            var objectList = document.getElementById("<%= ObjectList.ClientID %>");


            datasetList.disabled = true;
            objectTypeList.disabled = true;
            objectList.disabled = true;

            objectList.options.length = 0;
            var opt = new Option("Refreshing...", "");
            objectList.options.add(opt);
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_initializeRequest(ShowRefreshing);
    </script>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div class="toolbar">
                <div style="min-width: 140px">
                    <asp:Label ID="DatasetListLabel" runat="server" Text="Catalog:" /><br />
                    <asp:DropDownList ID="DatasetList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DatasetList_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div style="min-width: 140px">
                    <asp:Label ID="ObjectTypeListLabel" runat="server" Text="Object category:" /><br />
                    <asp:DropDownList ID="ObjectTypeList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ObjectTypeList_SelectedIndexChanged">
                        <asp:ListItem Selected="True" Value="Table">Tables</asp:ListItem>
                        <asp:ListItem Value="View">Views</asp:ListItem>
                        <asp:ListItem Value="StoredProcedure">Stored Procedures</asp:ListItem>
                        <asp:ListItem Value="ScalarFunction">Scalar Functions</asp:ListItem>
                        <asp:ListItem Value="TableValuedFunction">Table-valued Functions</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="span">
                    <asp:Label ID="ObjectListLabel" runat="server" Text="Object:" /><br />
                    <asp:DropDownList ID="ObjectList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ObjectList_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <asp:LinkButton runat="server" ID="summary" Text="summary"
                    OnCommand="ToolbarButton_Command" CommandName="summary" />
                <asp:LinkButton runat="server" ID="columns" Text="columns"
                    OnCommand="ToolbarButton_Command" CommandName="columns" />
                <asp:LinkButton runat="server" ID="indexes" Text="indexes"
                    OnCommand="ToolbarButton_Command" CommandName="indexes" />
                <asp:LinkButton runat="server" ID="parameters" Text="parameters"
                    OnCommand="ToolbarButton_Command" CommandName="parameters" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ContentPlaceHolderID="middle" runat="Server">
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <asp:Panel runat="server" ID="buttonPanel" Visible="false" CssClass="dock-bottom">
                <p class="FormButtons">
                    <asp:Button ID="peek" runat="server" Text="Peek" Visible="false" CssClass="FormButton" />
                    <asp:Button ID="export" runat="server" Text="Export" Visible="false" OnClick="Export_Click"
                        CssClass="FormButton" />
                    <asp:Button ID="rename" runat="server" Text="Rename" Visible="false" OnClick="Rename_Click"
                        CssClass="FormButton" />
                    <asp:Button ID="primaryKey" runat="server" Text="Primary Key" OnClick="PrimaryKey_Click"
                        CssClass="FormButton" />
                    <asp:Button ID="drop" runat="server" Text="Drop" Visible="false" OnClick="Drop_Click"
                        CssClass="FormButton" />
                </p>
            </asp:Panel>
            <list:SummaryForm ID="summaryForm" runat="server" Visible="false" />
            <list:ColumnList ID="columnList" runat="server" Visible="false" />
            <list:ParameterList ID="parameterList" runat="server" Visible="false" />
            <list:IndexList ID="indexList" runat="server" Visible="false" />
            <jgwuc:Form runat="server" ID="introForm" SkinID="Schema" Text="Getting started with the schema browser">
                <FormTemplate>
                    <ul>
                        <li>Select catalog from the first list. Catalogs are equivalent to databases. MyDB
                            referers to your own user data space.</li>
                        <li>Select object type from the second list. </li>
                        <li>Select object from the third list.</li>
                    </ul>
                </FormTemplate>
                <ButtonsTemplate>
                </ButtonsTemplate>
            </jgwuc:Form>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
