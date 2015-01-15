<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Domain.DomainForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="DomainForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm" runat="server">
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="ShortTitleLabel" runat="server" Text="Short Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="ShortTitle" runat="server" />
            </td>
        </tr>
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="LongTitleLabel" runat="server" Text="Long Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="LongTitle" runat="server" />
            </td>
        </tr>
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="EmailLabel" runat="server" Text="E-mail Address:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Email" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CopyrightLabel" runat="server" Text="Copyright:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Copyright" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DisclaimerLabel" runat="server" Text="Disclaimer:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Disclaimer" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="IdentityProviderLabel" runat="server" Text="Identity Provider:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="IdentityProvider" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AuthenticatorFactoryLabel" runat="server" Text="Authenticator Factory:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="AuthenticatorFactory" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>