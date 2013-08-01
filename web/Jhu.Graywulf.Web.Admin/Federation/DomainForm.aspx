<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.DomainForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="DomainForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm" runat="server">
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="ShortTitleLabel" runat="server" Text="Short Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="ShortTitle" runat="server" />
            </td>
        </tr>
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="LongTitleLabel" runat="server" Text="Long Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="LongTitle" runat="server" />
            </td>
        </tr>
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="EmailLabel" runat="server" Text="E-mail Address:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Email" runat="server" />
            </td>
        </tr>
        <tr runat="server" id="StandardUserGroupRow">
            <td class="FormLabel">
                <asp:Label ID="StandardUserGroupLabel" runat="server" Text="Standard User Group:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="StandardUserGroup" runat="server" CssClass="FormField">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>