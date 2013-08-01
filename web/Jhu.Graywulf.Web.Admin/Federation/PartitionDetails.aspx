<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.PartitionDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="PartitionDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FromLabel" runat="server" Text="From:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="From" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ToLabel" runat="server" Text="To:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="To" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>