<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Jobs.ExportJobDetails" CodeBehind="ExportJobDetails.aspx.cs" %>

<%@ Register Src="~/Jobs/Error.ascx" TagPrefix="jgwc" TagName="Error" %>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <div class="dock-top">
                <jgwc:MultiViewTabHeader runat="server" ID="tabs" MultiViewID="multiView" />
            </div>
            <div class="TabFrame dock-fill dock-container">
                <asp:MultiView runat="server" ID="multiView" ActiveViewIndex="0">
                    <jgwc:TabView runat="server" ID="summaryTab" Text="Summary">
                        <jgwc:Form runat="server" Text="Export job details" SkinID="ExportJobDetails">
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
                                            <asp:Label ID="Label2" runat="server" Text="Comments:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Comments" runat="server" Text="Label"></asp:Label>
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
                                            <jgwc:JobStatus runat="server" ID="JobExecutionStatus" />
                                        </td>
                                    </tr>
                                </table>
                            </FormTemplate>
                            <ButtonsTemplate>
                                <asp:Button runat="Server" ID="Download" Text="Download" CssClass="FormButton" />
                                <asp:Button runat="Server" ID="Cancel" Text="Cancel export" CssClass="FormButton" />
                                <asp:Button runat="Server" ID="Back" Text="Back" CssClass="FormButton" />
                            </ButtonsTemplate>
                        </jgwc:Form>
                    </jgwc:TabView>
                    <jgwc:TabView ID="exceptionTab" runat="server" Text="Error">
                        <jgwc:Error runat="server" ID="Error" />
                    </jgwc:TabView>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
