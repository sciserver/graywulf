<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Domain.UserIdentityForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="UserIdentityForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ProtocolLabel" runat="server" Text="Protocol:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="Protocol" runat="server" CssClass="FormField">
                    <asp:ListItem Value="Keystone" />
                    <asp:ListItem Value="OpenID" />
                    <asp:ListItem Value="OAuth" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AuthorityLabel" runat="server" Text="Authority:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Authority" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="IdentifierLabel" runat="server" Text="Identifier:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Identifier" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
