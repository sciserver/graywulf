<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.Default" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="JobList.ascx" TagPrefix="list" TagName="JobList" %>
<%-- %><%@ Register Src="QueryList.ascx" TagPrefix="list" TagName="QueryList" %>
<%@ Register Src="CopyList.ascx" TagPrefix="list" TagName="CopyList" %>
<%@ Register Src="ImportList.ascx" TagPrefix="list" TagName="ImportList" %>
<%@ Register Src="ExportList.ascx" TagPrefix="list" TagName="ExportList" %> --%>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <!--<meta http-equiv="Refresh" content="60" />-->
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
            <list:JobList runat="server" ID="jobList" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
