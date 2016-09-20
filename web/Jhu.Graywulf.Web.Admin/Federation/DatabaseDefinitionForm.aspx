<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.DatabaseDefinitionForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="DatabaseDefinitionForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm" runat="server">
        <tr runat="server" id="LayoutTypeRow">
            <td class="FormLabel">
                <asp:Label ID="LayoutTypeLabel" runat="server" Text="Layout Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="LayoutType" runat="server" CssClass="FormFieldNarrow" OnSelectedIndexChanged="LayoutType_SelectedIndexChanged"
                    AutoPostBack="true">
                    <asp:ListItem Value="Unknown" Text="(select type)" />
                    <asp:ListItem Value="Monolithic" Text="Monolithic" />
                    <asp:ListItem Value="Mirrored" Text="Mirrored" />
                    <asp:ListItem Value="Sliced" Text="Sliced" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr runat="server" id="DatabaseInstanceNamePatternRow">
            <td class="FormLabel">
                <asp:Label ID="DatabaseInstanceNamePatternLabel" runat="server" Text="DB Instance Name Pattern:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="DatabaseInstanceNamePattern" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr runat="server" id="DatabaseNamePatternRow">
            <td class="FormLabel">
                <asp:Label ID="DatabaseNamePatternLabel" runat="server" Text="DB Name Pattern:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="DatabaseNamePattern" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr runat="server" id="SliceCountRow" visible="false">
            <td class="FormLabel">
                <asp:Label ID="SliceCountLabel" runat="server" Text="Slice Count:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="SliceCount" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr runat="server" id="PartitionCountRow" visible="false">
            <td class="FormLabel">
                <asp:Label ID="PartitionCountLabel" runat="server" Text="Partition Count:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="PartitionCount" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr runat="server" id="PartitionRangeTypeRow" visible="false">
            <td class="FormLabel">
                <asp:Label ID="PartitionRangeTypeLabel" runat="server" Text="Range Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="PartitionRangeType" runat="server" CssClass="FormFieldNarrow">
                    <asp:ListItem Value="Unknown" Text="(select type)" />
                    <asp:ListItem Value="Left" Text="Left" />
                    <asp:ListItem Value="Right" Text="Right" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr runat="server" id="PartitionFunctionRow" visible="false">
            <td class="FormLabel">
                <asp:Label ID="PartitionFunctionLabel" runat="server" Text="Partition Function:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="PartitionFunction" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr runat="server" id="ServerVersionRow" visible="false">
            <td class="FormLabel">
                <asp:Label ID="ServerVersionLabel" runat="server" Text="Server Version:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="ServerVersion" runat="server" CssClass="FormField">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>