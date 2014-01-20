<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Domain.UserIdentityDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="UserIdentityDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ProtocolLabel" runat="server" Text="Protocol:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Protocol" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AuthorityLabel" runat="server" Text="Authority:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Authority" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="IdentifierLabel" runat="server" Text="Identifier:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Identifier" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
