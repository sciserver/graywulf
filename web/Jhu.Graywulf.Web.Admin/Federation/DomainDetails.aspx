<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.DomainDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="DomainDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ShortTitleLabel" runat="server" Text="Short Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ShortTitle" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="LongTitleLabel" runat="server" Text="Long Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="LongTitle" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="EmailLabel" runat="server" Text="Email:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Email" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="StandardUserGroupLabel" runat="server" Text="Standard User Group:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="StandardUserGroup" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="FederationList" ChildrenType="Federation" EntityGroup="Federation" />
    </jgwac:EntityChildren>
</asp:Content>

