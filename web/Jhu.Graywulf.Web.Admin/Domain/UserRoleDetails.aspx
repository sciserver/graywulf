<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Domain.UserRoleDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="UserRoleDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel"></asp:Label>
            </td>
            <td class="FormField">
                <asp:CheckBox ID="Default" runat="server" CssClass="FormField" Text="Default role" Enabled="false" />
            </td>
        </tr>
        </table>>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
