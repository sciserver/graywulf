<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.FileGroupDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="FileGroupDetails.aspx.cs" %>

<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FileGroupTypeLabel" runat="server" Text="Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="FileGroupType" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="LayoutTypeLabel" runat="server" Text="Layout Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="LayoutType" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AllocationTypeLabel" runat="server" Text="Allocation Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="AllocationType" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DiskVolumeTypeLabel" runat="server" Text="Disk Volume Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="DiskVolumeType" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FileGroupNameLabel" runat="server" Text="File Group Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="FileGroupName" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AllocatedSpaceLabel" runat="server" Text="AllocatedSpace"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="AllocatedSpace" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FileCountLabel" runat="server" Text="File Count:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="FileCount" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>