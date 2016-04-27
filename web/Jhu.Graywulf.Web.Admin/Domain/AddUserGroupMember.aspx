<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddUserGroupMember.aspx.cs" Inherits="Jhu.Graywulf.Web.Admin.Domain.AddUserGroupMember" MasterPageFile="~/Admin.master" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-fill dock-container" style="margin-top: 8px;">
        <h3>Make member of user group</h3>
        <table class="DetailsForm">
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="User Group:" /></td>
                <td class="FormField">
                    <asp:DropDownList runat="server" ID="UserGroup" CssClass="FormField" /></td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="User Role:" /></td>
                <td class="FormField">
                    <asp:DropDownList runat="server" ID="UserRole" CssClass="FormField" /></td>
            </tr>
        </table>
    </div>
    <div class="dock-bottom">
        <p class="Message">
            <asp:Label ID="Message" runat="server" Text="Label" Visible="False"></asp:Label>
        </p>
        <p class="FormButton">
            <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButton" OnClick="Ok_Click" />
            <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" OnClick="Cancel_Click"
                CausesValidation="False" />
        </p>
    </div>
</asp:Content>
