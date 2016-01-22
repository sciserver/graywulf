<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.ServerInstanceDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="ServerInstanceDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm dock-top">
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
                <asp:Label ID="InstanceNameLabel" runat="server" Text="Instance Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="InstanceName" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="IntegratedSecurityLabel" runat="server" Text="Security:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="IntegratedSecurity" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AdminUserLabel" runat="server" Text="Username:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="AdminUser" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" RunningStateButtonsVisible="true" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server" ID="EntityChildren">
        <jgwac:EntityList runat="server" ID="DiskGroupList" ChildrenType="ServerInstanceDiskGroup" EntityGroup="Cluster">
            <columns>
                <asp:BoundField DataField="DiskDesignation" HeaderText="Designation" />
            </columns>
        </jgwac:EntityList>
    </jgwac:EntityChildren>
</asp:Content>
