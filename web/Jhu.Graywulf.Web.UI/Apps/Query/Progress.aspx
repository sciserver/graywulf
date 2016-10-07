<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true" CodeBehind="Progress.aspx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Query.Progress" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <asp:UpdatePanel runat="server" class="dock-fill">
        <ContentTemplate>
            <asp:Panel runat="server" ID="statusPanel" CssClass="dock-fill">
                <jgwuc:Form runat="Server" Text="Executing query" SkinID="JobDetails">
                    <FormTemplate>
                        <table class="FormTable">
                            <tr>
                                <td class="FormLabel">
                                    <asp:Label runat="server" ID="statusLabel" Text="Query status:" />
                                </td>
                                <td>
                                    <jgwuc:JobStatus runat="server" ID="status" />
                                </td>
                            </tr>
                            <tr runat="server" id="exceptionRow" visible="false">
                                <td class="FormLabel">
                                    <asp:Label ID="exceptionLabel" runat="server" Text="Error:"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="exception" runat="server" Text="Label"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </FormTemplate>
                    <ButtonsTemplate>
                    </ButtonsTemplate>
                </jgwuc:Form>
                <asp:Timer runat="server" ID="statusTimer" Interval="2000" OnTick="StatusTimer_Tick" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
