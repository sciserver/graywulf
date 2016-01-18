<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.ServerInstanceDiskGroupDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="ServerInstanceDiskGroupDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm dock-top">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DiskGroupLabel" runat="server" Text="Disk Group:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="DiskGroup" runat="server" Expression="[$Parent.Name].[$Name]" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DiskDesignationLabel" runat="server" Text="Designation:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="DiskDesignation" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
