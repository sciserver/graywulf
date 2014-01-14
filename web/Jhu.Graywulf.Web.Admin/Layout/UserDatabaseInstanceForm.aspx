<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.UserDatabaseInstanceForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="UserDatabaseInstanceForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="UserLabel" runat="server" Text="User:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="UserList" runat="server" CssClass="FormField" AutoPostBack="True">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseInstanceLabel" runat="server" Text="Database Instance:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="DatabaseInstanceList" runat="server" CssClass="FormField">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>