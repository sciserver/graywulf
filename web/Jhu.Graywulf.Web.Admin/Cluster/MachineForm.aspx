<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.MachineForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="MachineForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="HostNameLabel" runat="server" Text="Host Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="HostName" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AdminUrlLabel" runat="server" Text="Admin URL:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="AdminUrl" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DeployUncPathLabel" runat="server" Text="Deployment Path:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="DeployUncPath" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
