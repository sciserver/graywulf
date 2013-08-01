<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.DatabaseInstanceFileForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="DatabaseInstanceFileForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DiskVolumeLabel" runat="server" Text="Disk Volume:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="DiskVolume" runat="server" CssClass="FormField">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseFileTypeLabel" runat="server" Text="File Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="DatabaseFileType" runat="server" CssClass="FormFieldNarrow">
                    <asp:ListItem Value="Unknown" Text="(select type)" />
                    <asp:ListItem Value="Data" Text="Data" />
                    <asp:ListItem Value="Log" Text="Log" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="LogicalNameLabel" runat="server" Text="Logical Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="LogicalName" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FilenameLabel" runat="server" Text="Filename:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Filename" runat="server" CssClass="FormField"></asp:TextBox>
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