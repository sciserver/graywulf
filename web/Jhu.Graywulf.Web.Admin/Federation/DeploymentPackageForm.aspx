<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.DeploymentPackageForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="DeploymentPackageForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="IndexNameLabel" runat="server" Text="Package file:"></asp:Label>
            </td>
            <td class="FormField">
                <input runat="server" id="Data" class="FormField" name="Data" type="file" />
            </td>
        </tr>
    </table>
    <p>
        Leave &#39;Package File&#39; field empty in order to keep existing file.</p>
</asp:Content>
