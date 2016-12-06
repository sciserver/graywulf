<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Tables" CodeBehind="Tables.aspx.cs" %>

<%@ Register Src="Toolbar.ascx" TagPrefix="jgwc" TagName="Toolbar" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <jgwc:Toolbar runat="server" ID="toolbar" SelectedTab="tables"
        OnSelectedDatasetChanged="Toolbar_SelectedDatasetChanged" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <div class="dock-top gw-list-frame-top">
        <div class="gw-list-header">
            <div class="gw-list-row">
                <span style="width: 32px"></span>
                <span class="gw-list-span">table name</span>
                <span style="width: 100px">row count</span>
                <span style="width: 100px">byte size</span>
                <span style="width: 140px">created</span>
                <span style="width: 140px">modified</span>
            </div>
        </div>
    </div>
    <div class="dock-bottom gw-list-frame-bottom">
        <div class="gw-list-footer">
            <div class="gw-list-row">
                <asp:DataPager runat="server" ID="listPager" PagedControlID="tableList" PageSize="20">
                    <Fields>
                        <asp:NextPreviousPagerField
                            ShowLastPageButton="false"
                            ShowNextPageButton="false" />
                        <asp:NumericPagerField ButtonCount="10" ButtonType="Link" />
                        <asp:NextPreviousPagerField
                            ShowFirstPageButton="false"
                            ShowPreviousPageButton="false" />
                    </Fields>
                </asp:DataPager>
            </div>
        </div>
    </div>
    <div class="gw-list-frame dock-fill">
        <jgwc:MultiSelectListView runat="server" ID="tableList"
            OnItemCreated="TableList_ItemCreated">
            <LayoutTemplate>
                <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
            </LayoutTemplate>
            <ItemTemplate>
                <div class="gw-list-item gw-details-container">
                    <div class="gw-list-row">
                        <jgwc:DetailsButton runat="server" Style="width: 32px" />
                        <asp:Label runat="server" Text='<%# Eval("DisplayName") %>' CssClass="gw-list-span" />
                        <asp:Label runat="server" Text='<%# Eval("Statistics.RowCount") %>' Width="100px" />
                        <jgwc:FancyByteSizeLabel runat="server" Value='<%# Eval("Statistics.DataSpace") %>' Width="100px" />
                        <jgwc:FancyDateLabel runat="server" Value='<%# Eval("Metadata.DateCreated") %>' Width="140px" />
                        <jgwc:FancyDateLabel runat="server" Value='<%# Eval("Metadata.DateModified") %>' Width="140px" />
                    </div>
                    <div class="gw-list-row gw-details-panel">
                        <span style="width: 32px"></span>
                        <span class="gw-list-span">
                            <asp:HyperLink runat="server" ID="schema" Text="schema" /> |
                            <asp:HyperLink runat="server" ID="peek" Text="peek" /> |
                            <asp:HyperLink runat="server" ID="export" Text="export" /> |
                            <asp:HyperLink runat="server" ID="rename" Text="rename" /> |
                            <asp:HyperLink runat="server" ID="copy" Text="copy" /> |
                            <asp:HyperLink runat="server" ID="primaryKey" Text="primary key" /> |
                            <asp:HyperLink runat="server" ID="drop" Text="drop" />
                        </span>
                    </div>
                </div>
            </ItemTemplate>
        </jgwc:MultiSelectListView>
    </div>
</asp:Content>
