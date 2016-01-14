<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.FileGroupForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="FileGroupForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FileGroupTypeLabel" runat="server" Text="Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="fileGroupTypeList" runat="server" CssClass="FormFieldNarrow">
                    <asp:ListItem Value="Unknown" Text="(select type)" />
                    <asp:ListItem Value="Data" Text="Data" />
                    <asp:ListItem Value="Log" Text="Log" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="LayoutTypeLabel" runat="server" Text="Layout Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="LayoutType" runat="server" CssClass="FormFieldNarrow">
                    <asp:ListItem Value="Unknown" Text="(select type)" />
                    <asp:ListItem Value="Monolithic" Text="Monolithic" />
                    <asp:ListItem Value="Sliced" Text="Sliced" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AllocationTypeLabel" runat="server" Text="Allocation Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="allocationTypeList" runat="server" CssClass="FormFieldNarrow">
                    <asp:ListItem Value="Unknown" Text="(select type)" />
                    <asp:ListItem Value="SingleVolume" Text="Single volume" />
                    <asp:ListItem Value="CrossVolume" Text="Cross-volume" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DiskDesignationLabel" runat="server" Text="Disk Designation:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="diskDesignationList" runat="server" CssClass="FormFieldNarrow">
                    <asp:ListItem Value="Unknown" Text="(select type)" />
                    <asp:ListItem Value="System" Text="System" />
                    <asp:ListItem Value="Data" Text="Data" />
                    <asp:ListItem Value="Log" Text="Log" />
                    <asp:ListItem Value="Temp" Text="Temporary" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FileGroupNameLabel" runat="server" Text="File Group Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="FileGroupName" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AllocatedSpaceName" runat="server" Text="Allocated Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="AllocatedSpace" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
                &nbsp;GB
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FileCountLabel" runat="server" Text="File Count:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="FileCount" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>