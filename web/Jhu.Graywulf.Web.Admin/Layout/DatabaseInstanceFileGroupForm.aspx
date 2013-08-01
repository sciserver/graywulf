<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.DatabaseInstanceFileGroupForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="DatabaseInstanceFileGroupForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FileGroupLabel" runat="server" Text="File Group:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="FileGroup" runat="server" CssClass="FormField">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="PartitionLabel" runat="server" Text="Partition:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="Partition" runat="server" CssClass="FormField">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AllocatedSpaceLabel" runat="server" Text="Allocated Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="AllocatedSpace" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="UsedSpaceLabel" runat="server" Text="Used Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="UsedSpace" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="RservedSpaceLabel" runat="server" Text="Reserved Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="ReservedSpace" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>