<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddUserRoleMember.aspx.cs" Inherits="Jhu.Graywulf.Web.Admin.Domain.AddUserRoleMember" MasterPageFile="~/Admin.master" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <h3>
        Make member of user group</h3>
    <p class="Message">
        <asp:Label ID="Message" runat="server" Text="Label" Visible="False"></asp:Label></p>
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel"><asp:Label runat="server" Text="User Role:" /></td>
            <td class="FormField"><asp:DropDownList runat="server" ID="UserRole" CssClass="FormField" /></td>
        </tr>
    </table>
    <p class="FormButton">
        <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButton" OnClick="Ok_Click" />
        <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" OnClick="Cancel_Click"
            CausesValidation="False" />
    </p>
</asp:Content>
