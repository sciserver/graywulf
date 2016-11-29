<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Toolbar.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Toolbar" %>
<asp:Panel runat="server" class="ToolbarFrame">
    <div class="Toolbar">
        <div class="ToolbarElement" style="width: 140px;">
            <asp:Label runat="server" ID="datasetListLabel" Text="Dataset:" /><br />
            <asp:DropDownList runat="server" ID="datasetList" CssClass="ToolbarControl" Style="width: 140px;"
                AutoPostBack="true" OnSelectedIndexChanged="DatasetList_SelectedIndexChanged" />
        </div>
        <div class="ToolbarElement" style="width: auto">
        </div>
    </div>
</asp:Panel>
