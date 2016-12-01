<%@ Control Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Controls.EntityForm" CodeBehind="EntityForm.ascx.cs" %>
<%@ Register TagPrefix="jgw" Namespace="Jhu.Graywulf.Web.Admin" Assembly="Jhu.Graywulf.Web.Admin" %>
<h3 class="DetailsTitle">
    <asp:Image ID="Icon" runat="server" />
    <asp:Label ID="OperationLabel" runat="server" Text="Label" /></h3>
<table class="DetailsForm">
    <tr>
        <td class="FormLabel">
            <asp:Label ID="nameLabel" runat="server" Text="Name:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:TextBox ID="name" runat="server" CssClass="FormField"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="displayNameLabel" runat="server" Text="Display name:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:TextBox ID="displayName" runat="server" CssClass="FormField"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="versionLabel" runat="server" Text="Version:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:TextBox ID="version" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label ID="commentsLabel" runat="server" Text="Comments:"></asp:Label>
        </td>
        <td class="FormField">
            <asp:TextBox ID="comments" runat="server" CssClass="FormField" Rows="5" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
</table>
