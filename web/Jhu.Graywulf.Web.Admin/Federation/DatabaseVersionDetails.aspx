<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.DatabaseVersionDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="DatabaseVersionDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ServerVersionLabel" runat="server" Text="Server Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="ServerVersion" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SizeMultiplierLabel" runat="server" Text="Size Multiplier:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="SizeMultiplier" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
