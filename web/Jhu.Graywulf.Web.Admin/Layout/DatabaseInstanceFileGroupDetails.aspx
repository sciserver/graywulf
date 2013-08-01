<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.DatabaseInstanceFileGroupDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="DatabaseInstanceFileGroupDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FileGroupLabel" runat="server" Text="File Group:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="FileGroup" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="PartitionLabel" runat="server" Text="Partition:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="Partition" runat="server" />
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
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren ID="EntityChildren1" runat="server">
        <jgwac:EntityList runat="server" ID="FileList" ChildrenType="DatabaseInstanceFile"
            EntityGroup="Layout" />
    </jgwac:EntityChildren>
</asp:Content>
