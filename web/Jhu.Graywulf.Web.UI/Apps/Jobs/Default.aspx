<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.Default" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="~/Apps/Jobs/ErrorDetails.ascx" TagPrefix="uc1" TagName="ErrorDetails" %>


<asp:Content runat="server" ContentPlaceHolderID="head">
    <meta http-equiv="Refresh" content="60" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div class="toolbar">
                <asp:LinkButton runat="server" ID="all" Text="all jobs"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="all" />
                <asp:LinkButton runat="server" ID="query" Text="query jobs"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="query" />
                <asp:LinkButton runat="server" ID="copy" Text="copy jobs"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="copy" />
                <asp:LinkButton runat="server" ID="export" Text="export jobs"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="export" />
                <asp:LinkButton runat="server" ID="import" Text="import jobs"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="import" />
                <asp:LinkButton runat="server" ID="script" Text="script jobs"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="script" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ContentPlaceHolderID="middle" runat="Server">
    <asp:ObjectDataSource runat="server" ID="JobDataSource" EnableViewState="true" EnablePaging="true"
        OnObjectCreating="JobDataSource_ObjectCreating" SelectCountMethod="CountJobs"
        SelectMethod="SelectJobs" TypeName="Jhu.Graywulf.Web.Api.V1.JobFactory" StartRowIndexParameterName="from"
        MaximumRowsParameterName="max" EnableCaching="false" />
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <div class="dock-top gw-list-frame-top">
                <div class="gw-list-header">
                    <div class="gw-list-row">
                        <!--<span style="width: 32px"></span>-->
                        <span style="width: 100px"></span>
                        <span style="width: 32px"></span>
                        <span style="width: 64px">job type</span>
                        <span style="width: 64px">queue</span>
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
                        <asp:DataPager runat="server" ID="listPager" PagedControlID="list" PageSize="20">
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
                                <jgwc:DetailsButton runat="server" Style="width: 32px" />
                                <asp:Label runat="server" Text='<%# Eval("Type") %>' Width="64px" />
                                <asp:Label runat="server" Text='<%# Eval("Queue") %>' Width="64px" />
                                <jgwc:FancyDateLabel runat="server" Value='<%# Eval("DateCreated") %>' Width="140px" />
                                <jgwc:FancyDateLabel runat="server" Value='<%# Eval("DateStarted") %>' Width="140px" />
                                <jgwc:FancyDateLabel runat="server" Value='<%# Eval("DateFinished") %>' Width="140px" />
                                <asp:Label runat="server" Text='<%# Eval("Comments") %>' CssClass="gw-list-span" />
                            </div>
                            <asp:PlaceHolder runat="server" ID="detailsPlaceholder" />
                            <uc1:ErrorDetails runat="server" id="errorDetails" />
                        </div>
                    </ItemTemplate>
                </jgwc:MultiSelectListView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
