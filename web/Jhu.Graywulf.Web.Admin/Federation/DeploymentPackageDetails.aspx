<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.DeploymentPackageDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="DeploymentPackageDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FilenameLabel" runat="server" Text="Filename:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Filename" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
