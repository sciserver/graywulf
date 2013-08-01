<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.DatabaseInstanceFileDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="DatabaseInstanceFileDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DiskVolumeLabel" runat="server" Text="Disk Volume:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="DiskVolume" Expression="[$Machine.Name].[$Name]" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseFileTypeLabel" runat="server" Text="File Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="DatabaseFileType" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="LogicalNameLabel" runat="server" Text="Logical Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="LogicalName" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FilenameLabel" runat="server" Text="Filename:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:HyperLink ID="filename" runat="server" Text="Label" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AllocatedSpaceLabel" runat="server" Text="Allocated Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="AllocatedSpace" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="UsedSpaceLabel" runat="server" Text="Used Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="UsedSpace" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ReservedSpaceLabel" runat="server" Text="Reserved Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ReservedSpace" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>

