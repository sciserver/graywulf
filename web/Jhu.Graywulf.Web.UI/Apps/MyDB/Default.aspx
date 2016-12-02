<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="True"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Default" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="Toolbar.ascx" TagPrefix="jgwc" TagName="Toolbar" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <jgwc:Toolbar runat="server" ID="toolbar" SelectedTab="summary" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <jgwuc:Form runat="server" Text="User database summary" SkinID="MyDB">
        <FormTemplate>
            <p>
                The following is a summary of your '<asp:Label runat="server" ID="datasetName" />' usage:
            </p>
            <table class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label runat="server" ID="datasetUsageLabel" />
                        usage:
                    </td>
                    <td class="FormField">
                        <table style="width: 100%">
                            <tr>
                                <td runat="server" id="usageUsed" style="background-color: Red">&nbsp;
                                </td>
                                <td runat="server" id="usageFree" style="background-color: Green">&nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel"></td>
                    <td class="FormField">
                        <asp:Label runat="server" ID="ProgressUsedLabel" />
                        free
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">Space allocated:
                    </td>
                    <td class="FormField">
                        <asp:Label runat="server" ID="DataSpace" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">Space used:
                    </td>
                    <td class="FormField">
                        <asp:Label runat="server" ID="UsedSpace" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">Log space allocated:
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
    </jgwuc:Form>
</asp:Content>
