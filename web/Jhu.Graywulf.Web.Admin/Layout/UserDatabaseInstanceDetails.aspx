<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.UserDatabaseInstanceDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="UserDatabaseInstanceDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="UserLabel" runat="server" Text="User:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="User" Expression="[$Name]" runat="server" />
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

