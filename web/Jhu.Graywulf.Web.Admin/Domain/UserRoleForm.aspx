<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Domain.UserRoleForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="UserRoleForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel"></asp:Label>
            </td>
            <td class="FormField">
                <asp:CheckBox ID="Default" runat="server" CssClass="FormField" Text="Default role" />
            </td>
        </tr>
    </table>
</asp:Content>