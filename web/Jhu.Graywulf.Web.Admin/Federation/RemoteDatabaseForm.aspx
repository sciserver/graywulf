<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.RemoteDatabaseForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="RemoteDatabaseForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ProviderNameLabel" runat="server" Text="Provider Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="ProviderName" runat="server" CssClass="FormFieldNarrow">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ConnectionStringLabel" runat="server" Text="Connection String:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox runat="server" ID="ConnectionString" CssClass="FormField" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="IntegratedSecurityLabel" runat="server" Text="Authentication:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:CheckBox ID="IntegratedSecurity" runat="server" Text="Integrated Security" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="UsernameLabel" runat="server" Text="Username:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Username" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="PasswordLabel" runat="server" Text="Password:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Password" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="RequiresSshTunnelLabel" runat="server" Text="Tunneling:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:CheckBox ID="RequiresSshTunnel" runat="server" Text="RequiresSshTunnel" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SshHostNameLabel" runat="server" Text="SSH host name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="SshHostName" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SshPortNumberLabel" runat="server" Text="SSH port number:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="SshPortNumber" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SshUsernameLabel" runat="server" Text="SSH username:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="SshUsername" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SshPasswordLabel" runat="server" Text="SSH password:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="SshPassword" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
