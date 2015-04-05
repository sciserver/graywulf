<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Jobs.JobDetails" CodeBehind="JobDetails.aspx.cs" %>

<%@ Register Src="~/Jobs/ErrorForm.ascx" TagPrefix="jgwc" TagName="ErrorForm" %>
<%@ Register Src="~/Jobs/QueryForm.ascx" TagPrefix="jgwc" TagName="QueryForm" %>
<%@ Register Src="~/Jobs/ExportForm.ascx" TagPrefix="jgwc" TagName="ExportForm" %>
<%@ Register Src="~/Jobs/ImportForm.ascx" TagPrefix="jgwc" TagName="ImportForm" %>



<asp:Content runat="server" ContentPlaceHolderID="head">
    <link rel="stylesheet" href="../Scripts/CodeMirror/lib/codemirror.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <div class="dock-top">
                <jgwc:MultiViewTabHeader runat="server" ID="tabs" MultiViewID="multiView" />
            </div>
            <div class="TabFrame dock-fill dock-container">
                <asp:MultiView runat="server" ID="multiView" ActiveViewIndex="0">
                    <jgwc:TabView runat="server" ID="summaryTab" Text="Summary">
                        <jgwc:Form runat="Server" Text="Job details" SkinID="JobDetails">
                            <FormTemplate>
                                <table class="FormTable">
                                    <tr>
                                        <td class="FormLabel">
                                            <asp:Label ID="Label1" runat="server" Text="Job ID:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Name" runat="server" Text="Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="FormLabel">
                                            <asp:Label ID="Label3" runat="server" Text="Submitted:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="DateCreated" runat="server" Text="Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="FormLabel">
                                            <asp:Label ID="Label6" runat="server" Text="Started:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="DateStarted" runat="server" Text="Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="FormLabel">
                                            <asp:Label ID="Label7" runat="server" Text="Finished:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="DateFinished" runat="server" Text="Label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="FormLabel">
                                            <asp:Label ID="Label4" runat="server" Text="Status:"></asp:Label>
                                        </td>
                                        <td>
                                            <jgwuc:JobStatus runat="server" ID="JobExecutionStatus" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="FormLabel">
                                            <asp:Label ID="Label2" runat="server" Text="Comments:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Comments" runat="server" Text="Label"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </FormTemplate>
                            <ButtonsTemplate>
                                <asp:Button runat="Server" ID="Cancel" Text="Cancel job" CssClass="FormButton"
                                    OnClick="Cancel_Click" />
                                <asp:Button runat="Server" ID="Back" Text="Back" CssClass="FormButton" />
                            </ButtonsTemplate>
                        </jgwc:Form>
                    </jgwc:TabView>
                    <jgwc:TabView ID="queryTab" runat="server" Text="Query">
                        <jgwc:QueryForm runat="server" ID="queryForm" />
                    </jgwc:TabView>
                    <jgwc:TabView ID="exportTab" runat="server" Text="Export">
                        <jgwc:ExportForm runat="server" ID="exportForm" />
                    </jgwc:TabView>
                    <jgwc:TabView ID="importTab" runat="server" Text="Import">
                        <jgwc:ImportForm runat="server" id="importForm" />
                    </jgwc:TabView>
                    <jgwc:TabView ID="errorTab" runat="server" Text="Error">
                        <jgwc:ErrorForm runat="server" ID="ErrorForm" />
                    </jgwc:TabView>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
