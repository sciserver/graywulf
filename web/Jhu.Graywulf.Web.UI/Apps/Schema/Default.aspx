<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.Default" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="ColumnsView.ascx" TagName="Columns" TagPrefix="view" %>
<%@ Register Src="DatabaseObjectView.ascx" TagName="DatabaseObject" TagPrefix="view" %>
<%@ Register Src="DatasetsView.ascx" TagName="Datasets" TagPrefix="view" %>
<%@ Register Src="DatasetView.ascx" TagName="Dataset" TagPrefix="view" %>
<%@ Register Src="IndexesView.ascx" TagName="Indexes" TagPrefix="view" %>
<%@ Register Src="ParametersView.ascx" TagName="Parameters" TagPrefix="view" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <script>
        function ShowRefreshing() {
            var datasetList = document.getElementById("<%= datasetList.ClientID %>");
            var objectTypeList = document.getElementById("<%= objectTypeList.ClientID %>");
            var objectList = document.getElementById("<%= databaseObjectList.ClientID %>");

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
                    <asp:Label ID="datasetListLabel" runat="server" Text="Catalog:" /><br />
                    <asp:DropDownList ID="datasetList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DatasetList_SelectedIndexChanged" />
                </div>
                <div style="min-width: 140px">
                    <asp:Label ID="objectTypeListLabel" runat="server" Text="Object category:" /><br />
                    <asp:DropDownList ID="objectTypeList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ObjectTypeList_SelectedIndexChanged" />
                </div>
                <div class="span">
                    <asp:Label ID="databaseObjectListLabel" runat="server" Text="Object:" /><br />
                    <asp:DropDownList ID="databaseObjectList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DatabaseObjectList_SelectedIndexChanged"/>
                </div>
                <asp:LinkButton runat="server" ID="summaryButton" Text="summary" OnCommand="ViewButton_Command" CommandName="Summary" />
                <asp:LinkButton runat="server" ID="columnsButton" Text="columns" OnCommand="ViewButton_Command" CommandName="Columns" />
                <asp:LinkButton runat="server" ID="indexesButton" Text="indexes" OnCommand="ViewButton_Command" CommandName="Indexes" />
                <asp:LinkButton runat="server" ID="parametersButton" Text="parameters" OnCommand="ViewButton_Command" CommandName="Parameters" />
                <asp:HyperLink runat="server" ID="peekButton" Text="peek" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ContentPlaceHolderID="middle" runat="Server">
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <view:Datasets ID="datasetsView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:Dataset ID="datasetView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:DatabaseObject ID="databaseObjectView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:Columns ID="columnsView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:Parameters ID="parametersView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:Indexes ID="indexesView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
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
