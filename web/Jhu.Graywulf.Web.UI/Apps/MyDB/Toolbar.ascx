<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Toolbar.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Toolbar" %>

<%@ Register Src="~/Apps/MyDB/DatasetList.ascx" TagPrefix="jgwu" TagName="DatasetList" %>

<asp:Panel runat="server" class="ToolbarFrame">
    <div class="Toolbar">
        <div class="ToolbarElement" style="width: 140px;">
            <asp:Label runat="server" ID="datasetListLabel" Text="Dataset:" /><br />
            <jgwu:DatasetList runat="server" ID="datasetList" CssClass="ToolbarControl" Style="width: 140px;"
                AutoPostBack="true" OnSelectedDatasetChanged="DatasetList_SelectedDatasetChanged" />
        </div>
        <div class="ToolbarElement" style="width: auto">
        </div>
    </div>
</asp:Panel>
