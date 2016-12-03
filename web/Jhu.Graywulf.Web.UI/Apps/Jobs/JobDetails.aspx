<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.JobDetails" CodeBehind="JobDetails.aspx.cs" %>

<%@ Register Src="JobForm.ascx" TagPrefix="jgwc" TagName="JobForm" %>
<%@ Register Src="ErrorForm.ascx" TagPrefix="jgwc" TagName="ErrorForm" %>
<%@ Register Src="QueryForm.ascx" TagPrefix="jgwc" TagName="QueryForm" %>
<%@ Register Src="ExportForm.ascx" TagPrefix="jgwc" TagName="ExportForm" %>
<%@ Register Src="ImportForm.ascx" TagPrefix="jgwc" TagName="ImportForm" %>
<%@ Register Src="CopyForm.ascx" TagPrefix="jgwc" TagName="CopyForm" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div class="toolbar">
                <asp:LinkButton runat="server" ID="back" Text="back" CausesValidation="false"
                    OnClick="Back_Click" />
                <asp:LinkButton runat="server" ID="summary" Text="summary"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="summary" />
                <asp:LinkButton runat="server" ID="query" Text="query"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="query" />
                <asp:LinkButton runat="server" ID="copy" Text="details"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="copy" />
                <asp:LinkButton runat="server" ID="export" Text="details"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="export" />
                <asp:LinkButton runat="server" ID="import" Text="details"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="import" />
                <asp:LinkButton runat="server" ID="error" Text="error"
                    CausesValidation="false" OnCommand="ToolbarButton_Command" CommandName="error" />
                <asp:LinkButton runat="server" ID="cancel" Text="cancel" CausesValidation="false"
                    OnClick="Cancel_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <jgwc:JobForm runat="server" ID="jobForm" />
            <jgwc:QueryForm runat="server" ID="queryForm" />
            <jgwc:ExportForm runat="server" ID="exportForm" />
            <jgwc:ImportForm runat="server" ID="importForm" />
            <jgwc:CopyForm runat="server" ID="copyForm" />
            <jgwc:ErrorForm runat="server" ID="errorForm" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
