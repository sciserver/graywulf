<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.UserDatabaseInstanceForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="UserDatabaseInstanceForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FederationLabel" runat="server" Text="Federation:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="Federation" runat="server" CssClass="FormField" AutoPostBack="True"
                    OnSelectedIndexChanged="Federation_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseVersionLabel" runat="server" Text="Database Version:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="DatabaseVersion" runat="server" CssClass="FormField" AutoPostBack="True"
                    OnSelectedIndexChanged="DatabaseVersion_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseInstanceLabel" runat="server" Text="Database Instance:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="DatabaseInstance" runat="server" CssClass="FormField">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>