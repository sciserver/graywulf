<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.UserDatabaseInstanceDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="UserDatabaseInstanceDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FederationLabel" runat="server" Text="Federation:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="Federation" Expression="[$Name]" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseDefinitionLabel" runat="server" Text="Database Definition:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="DatabaseDefinition" Expression="[$Name]" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseInstanceLabel" runat="server" Text="Database Instance:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="DatabaseInstance" Expression="[$Name]" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>

