<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" Async="true" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.Default" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="ColumnsView.ascx" TagName="Columns" TagPrefix="view" %>
<%@ Register Src="DatabaseObjectView.ascx" TagName="DatabaseObject" TagPrefix="view" %>
<%@ Register Src="DatabaseObjectListView.ascx" TagName="DatabaseObjectList" TagPrefix="view" %>
<%@ Register Src="DatasetListView.ascx" TagName="DatasetList" TagPrefix="view" %>
<%@ Register Src="DatasetView.ascx" TagName="Dataset" TagPrefix="view" %>
<%@ Register Src="IndexesView.ascx" TagName="Indexes" TagPrefix="view" %>
<%@ Register Src="ParametersView.ascx" TagName="Parameters" TagPrefix="view" %>
<%@ Register Src="Peek.ascx" TagName="Peek" TagPrefix="view" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div class="toolbar">
                <asp:LinkButton runat="server" ID="datasetListButton" Text="schema home" OnCommand="SchemaView_Command" CommandName="datasetlist" />
                <div runat="server" id="datasetListDiv" style="min-width: 140px">
                    <asp:Label ID="datasetListLabel" runat="server" Text="Data set:" /><br />
                    <asp:DropDownList ID="datasetList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DatasetList_SelectedIndexChanged" />
                </div>
                <asp:LinkButton runat="server" ID="datasetButton" Text="data set" OnCommand="SchemaView_Command" CommandName="dataset" />
                <div runat="server" id="objectTypeListDiv" style="min-width: 140px">
                    <asp:Label ID="objectTypeListLabel" runat="server" Text="Object category:" /><br />
                    <asp:DropDownList ID="objectTypeList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ObjectTypeList_SelectedIndexChanged" />
                </div>
                <asp:LinkButton runat="server" ID="objectListButton" Text="object list" OnCommand="SchemaView_Command" CommandName="databaseObjectList" />
                <div runat="server" id="databaseObjectListDiv" class="span">
                    <asp:Label ID="databaseObjectListLabel" runat="server" Text="Object:" /><br />
                    <asp:DropDownList ID="databaseObjectList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DatabaseObjectList_SelectedIndexChanged" />
                </div>
                <div runat="server" id="toolbarSpan" class="span"></div>
                <asp:LinkButton runat="server" ID="summaryButton" Text="summary" OnCommand="ViewButton_Command" CommandName="DatabaseObject" />
                <asp:LinkButton runat="server" ID="parametersButton" Text="parameters" OnCommand="ViewButton_Command" CommandName="Parameters" />
                <asp:LinkButton runat="server" ID="columnsButton" Text="columns" OnCommand="ViewButton_Command" CommandName="Columns" />
                <asp:LinkButton runat="server" ID="indexesButton" Text="indexes" OnCommand="ViewButton_Command" CommandName="Indexes" />
                <asp:LinkButton runat="server" ID="peekButton" Text="peek" OnCommand="ViewButton_Command" CommandName="Peek" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ContentPlaceHolderID="middle" runat="Server">
    <asp:UpdateProgress runat="server" DisplayAfter="150">
        <ProgressTemplate>
            <div class="LayoutProgress">
                <asp:Image runat="server" SkinID="Progress" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <view:DatasetList ID="datasetListView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:Dataset ID="datasetView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:DatabaseObject ID="databaseObjectView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:DatabaseObjectList ID="databaseObjectListView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:Columns ID="columnsView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:Parameters ID="parametersView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:Indexes ID="indexesView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
            <view:Peek ID="peekView" runat="server" Visible="false" OnCommand="SchemaView_Command" />
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
