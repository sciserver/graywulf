<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.JobDefinitionForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="JobDefinitionForm.aspx.cs" ValidateRequest="false" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="Form">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TypeNameLabel" runat="server" Text="Type Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="TypeName" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
