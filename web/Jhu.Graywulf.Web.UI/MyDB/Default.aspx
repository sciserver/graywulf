<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="True"
    Inherits="Jhu.Graywulf.Web.UI.MyDB.Default" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="~/MyDb/Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-top">
        <jgwc:MyDbTabs ID="MyDbTabs1" runat="server" SelectedTab="Summary" />
    </div>
    <div class="dock-fill dock-container TabFrame">
        <jgwc:Form runat="server" Text="MyDB Summary" SkinID="MyDB">
            <FormTemplate>
                <p>
                    The following is a summary of your MyDB usage.
                </p>
                <table class="FormTable">
                    <tr>
                        <td class="FormLabel">
                            MyDB Usage:
                        </td>
                        <td class="FormField">
                            <table style="width: 100%">
                                <tr>
                                    <td runat="server" id="ProgressUsed" style="background-color: Red">
                                        &nbsp;
                                    </td>
                                    <td runat="server" id="ProgressFree" style="background-color: Green">
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                        </td>
                        <td class="FormField">
                            <asp:Label runat="server" ID="ProgressUsedLabel" />
                            free
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            Space allocated:
                        </td>
                        <td class="FormField">
                            <asp:Label runat="server" ID="DataSpace" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            Space used:
                        </td>
                        <td class="FormField">
                            <asp:Label runat="server" ID="UsedSpace" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            Log space allocated:
                        </td>
                        <td class="FormField">
                            <asp:Label runat="server" ID="LogSpace" />
                        </td>
                    </tr>
                </table>
                <ul>
                    <li>Running out of space? You can
                        <asp:HyperLink runat="server" ID="RequestSpaceLink">request more</asp:HyperLink>.</li>
                </ul>
            </FormTemplate>
            <ButtonsTemplate>
            </ButtonsTemplate>
        </jgwc:Form>
    </div>
</asp:Content>
