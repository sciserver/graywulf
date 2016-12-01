<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CopyForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.CopyForm" %>
<jgwuc:Form runat="server" Text="Import details" SkinID="CopyJobDetails">
    <FormTemplate>
        <table class="FormTable">
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="Source dataset:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label runat="server" ID="sourceDataset"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="Source table:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label runat="server" ID="sourceTable"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="Destination dataset:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label runat="server" ID="destinationDataset"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="Destination table:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label runat="server" ID="destinationTable"></asp:Label>
                </td>
            </tr>
        </table>
    </FormTemplate>
    <ButtonsTemplate>
    </ButtonsTemplate>
</jgwuc:Form>
