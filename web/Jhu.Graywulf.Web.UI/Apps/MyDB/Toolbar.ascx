<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Toolbar.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Toolbar" %>

<%@ Register Src="~/Apps/MyDB/DatasetList.ascx" TagPrefix="jgwu" TagName="DatasetList" %>

<div runat="server" id="toolbar" class="toolbar">
    <div style="min-width: 140px;">
        <asp:Label runat="server" ID="datasetListLabel" Text="Dataset:" /><br />
        <jgwu:DatasetList runat="server" ID="datasetList" CssClass="ToolbarControl" Style="width: 140px;"
            AutoPostBack="true" CausesValidation="false"
            OnSelectedDatasetChanged="DatasetList_SelectedDatasetChanged" />
    </div>
    <asp:LinkButton runat="server" ID="summary" OnCommand="Button_Command" CommandName="summary"
        CausesValidation="false" Text="summary" />
    <asp:LinkButton runat="server" ID="tables" OnCommand="Button_Command" CommandName="tables"
        CausesValidation="false" Text="tables" />
    <asp:LinkButton runat="server" ID="copy" OnCommand="Button_Command" CommandName="copy"
        CausesValidation="false" Text="copy" />
    <asp:LinkButton runat="server" ID="export" OnCommand="Button_Command" CommandName="export"
        CausesValidation="false" Text="export" />
    <asp:LinkButton runat="server" ID="import" OnCommand="Button_Command" CommandName="import"
        CausesValidation="false" Text="import" />
    <div class="span"></div>
</div>