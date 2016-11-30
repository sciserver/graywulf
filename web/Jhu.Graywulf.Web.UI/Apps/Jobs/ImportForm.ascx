<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.ImportForm" %>
<jgwuc:Form runat="server" Text="Import details" SkinID="ImportJobDetails">
    <FormTemplate>
        <table class="FormTable">
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="URL:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label runat="server" ID="uri"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="File format:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label runat="server" ID="fileFormat"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="Dataset:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label runat="server" ID="dataset"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="Table:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label runat="server" ID="table"></asp:Label>
                </td>
            </tr>
        </table>
    </FormTemplate>
    <ButtonsTemplate>
    </ButtonsTemplate>
</jgwuc:Form>
