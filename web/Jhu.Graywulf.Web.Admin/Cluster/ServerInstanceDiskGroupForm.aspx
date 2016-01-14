<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.ServerInstanceDiskGroupForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="ServerInstanceDiskGroupForm.aspx.cs" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DiskGroupLabel" runat="server" Text="Disk Group:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="DiskGroup" runat="server" CssClass="FormField">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DiskDesignationLabel" runat="server" Text="Designation:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="DiskDesignation" runat="server">
                    <asp:ListItem Value="Unknown"></asp:ListItem>
                    <asp:ListItem Value="System"></asp:ListItem>
                    <asp:ListItem Value="Data"></asp:ListItem>
                    <asp:ListItem Value="Log"></asp:ListItem>
                    <asp:ListItem Value="Temp"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>
