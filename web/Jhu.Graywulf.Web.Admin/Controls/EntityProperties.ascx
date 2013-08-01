<%@ Control Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Controls.EntityProperties"
    CodeBehind="EntityProperties.ascx.cs" %>
<%@ Register TagPrefix="jgwc" TagName="EntityLink" Src="~/Controls/EntityLink.ascx" %>
<table class="DetailsForm dock-top">
    <tr>
        <td class="FormLabel">
            <asp:Label ID="FullyQualifiedNameLabel" runat="server" Text="Fully Qualified Name:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:Label ID="FullyQualifiedName" runat="server" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="EntityGuidLabel" runat="server" Text="Guid:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:Label ID="EntityGuid" runat="server" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="VersionLabel" runat="server" Text="Version:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:Label ID="Version" runat="server" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="DeploymentStateLabel" runat="server" Text="Deployment State:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:Label ID="DeploymentState" runat="server" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="RunningStateLabel" runat="server" Text="Running State:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:Label ID="RunningState" runat="server" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="AlertStateLabel" runat="server" Text="Alert State:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:Label ID="AlertState" runat="server" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="CommentsLabel" runat="server" Text="Comments:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:Label ID="Comments" runat="server" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="AttributesLabel" runat="server" Text="Attributes:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:CheckBox ID="System" runat="server" Enabled="False" Text="System" />
            <asp:CheckBox ID="Hidden" runat="server" Enabled="False" Text="Hidden" />
            <asp:CheckBox ID="ReadOnly" runat="server" Enabled="False" Text="Read-only" />
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
        </td>
        <td class="FormField">
            <asp:CheckBox ID="Locked" runat="server" Enabled="False" Text="Locked" />
            <asp:CheckBox ID="Primary" runat="server" Enabled="False" Text="Primary" />
            <asp:CheckBox ID="Deleted" runat="server" Enabled="False" Text="Deleted" />
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="OwnerLabel" runat="server" Text="Owner:"></asp:Label>
        </td>
        <td class="FormField">
            <jgwc:EntityLink ID="UserOwner" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="CreatedLabel" runat="server" Text="Created:" />
        </td>
        <td class="FormField">
            <jgwc:EntityLink ID="UserCreated" runat="server" /> at
            <asp:Label ID="DateCreated" runat="server" Text="Label" />
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="ModifiedLabel" runat="server" Text="Modified:"></asp:Label>
        </td>
        <td class="FormField">
            <jgwc:EntityLink ID="UserModified" runat="server" /> at
            <asp:Label ID="DateModified" runat="server" Text="Label" />
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="DeletedLabel" runat="server" Text="Deleted:"></asp:Label>
        </td>
        <td class="FormField">
            <jgwc:EntityLink ID="UserDeleted" runat="server" /> at
            <asp:Label ID="DateDeleted" runat="server" Text="Label" />
        </td>
    </tr>
</table>
