<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.RemoteDatabaseDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="RemoteDatabaseDetails.aspx.cs" %>

<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ProviderNameLabel" runat="server" Text="Provider Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ProviderName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ConnectionStringLabel" runat="server" Text="Connection String:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ConnectionString" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="IntegratedSecurityLabel" runat="server" Text="Integrated Security:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="IntegratedSecurity" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="UsernameLabel" runat="server" Text="Username:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Username" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="PasswordLabel" runat="server" Text="Password:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Password" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="RequiresSshTunnelLabel" runat="server" Text="Requires SSH Tunnel:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="RequiresSshTunnel" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SshHostNameLabel" runat="server" Text="SSH Host Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="SshHostName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SshPortNumberLabel" runat="server" Text="SSH Port Number:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="SshPortNumber" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SshUsernameLabel" runat="server" Text="SSH Username:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="SshUsername" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SshPasswordLabel" runat="server" Text="SSH Password:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="SshPassword" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
