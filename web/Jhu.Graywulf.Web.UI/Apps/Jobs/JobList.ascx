<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobList.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.AllList" %>

<div class="dock-top gw-list-frame-top">
    <div class="gw-list-header">
        <div class="gw-list-row">
            <!--<span style="width: 32px"></span>-->
            <span style="width: 100px"></span>
            <span style="width: 32px"></span>
            <span style="width: 64px">job type</span>
            <span style="width: 140px">submitted</span>
            <span style="width: 140px">started</span>
            <span style="width: 140px">finished</span>
            <span class="gw-list-span">comments</span>
        </div>
    </div>
</div>
<div class="dock-bottom gw-list-frame-bottom">
    <div class="gw-list-footer">
        <div class="gw-list-row">
            <asp:DataPager runat="server" ID="listPager" PagedControlID="list" PageSize="10">
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
    <jgwc:MultiSelectListView runat="server" ID="list" SelectionCheckboxID="selection"
        SelectionElementID="listItem" CssClassSelected="gw-list-item-selected"
        OnItemCreated="List_ItemCreated">
        <LayoutTemplate>
            <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
        </LayoutTemplate>
        <ItemTemplate>
            <div runat="server" id="listItem" class="gw-list-item gw-details-container">
                <div class="gw-list-row">
                    <!--<span style="width: 32px">
                        <asp:CheckBox runat="server" ID="selection" /></span>-->
                    <jgwuc:JobStatus runat="server" Status='<%# Eval("Status") %>' Width="100px" Style="text-align: center" />
                    <jgwc:DetailsButton runat="server" style="width: 32px" />
                    <asp:Label runat="server" Text='<%# Eval("Type") %>' Width="64px" />
                    <jgwc:FancyDateLabel runat="server" Value='<%# Eval("DateCreated") %>' Width="140px" />
                    <jgwc:FancyDateLabel runat="server" Value='<%# Eval("DateStarted") %>' Width="140px" />
                    <jgwc:FancyDateLabel runat="server" Value='<%# Eval("DateFinished") %>' Width="140px" />
                    <asp:Label runat="server" Text='<%# Eval("Comments") %>' CssClass="gw-list-span" />
                </div>
                <div runat="server" id="errorDiv" class="gw-list-row gw-details-panel" style="display: none;">
                    <span style="width: 132px"></span>
                    <span style="width: 100px; text-align: left">Error:</span>
                    <asp:Label runat="server" ID="errorText" CssClass="gw-list-span" />
                </div>
                <div runat="server" id="buttonDiv" class="gw-list-row gw-details-panel" style="display: none;">
                    <span style="width: 132px"></span>
                    <asp:Button runat="server" ID="details" Text="more details"/>&nbsp;
                    <asp:Button runat="server" ID="cancel" Text="cancel job"/>
                </div>
                <asp:PlaceHolder runat="server" ID="detailsPlaceholder" />
            </div>
        </ItemTemplate>
    </jgwc:MultiSelectListView>
</div>
