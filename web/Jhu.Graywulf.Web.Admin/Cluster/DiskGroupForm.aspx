<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.DiskGroupForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="DiskGroupForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TypeLabel" runat="server" Text="Volume Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="Type" runat="server">
                    <asp:ListItem Value="Jbod"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FullSpaceLabel" runat="server" Text="Full Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="FullSpace" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
                <asp:RequiredFieldValidator ID="FullSpaceRequiredValidator" runat="server" ControlToValidate="FullSpace"
                    Display="Dynamic" ErrorMessage="&lt;br&gt;This field is required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AllocatedSpaceLabel" runat="server" Text="Allocated Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="AllocatedSpace" runat="server" CssClass="FormFieldNarrow" Enabled="False"></asp:TextBox>
                <asp:RequiredFieldValidator ID="AllocatedSpaceRequiredValidator" runat="server" ControlToValidate="AllocatedSpace"
                    Display="Dynamic" ErrorMessage="&lt;br&gt;This field is required"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ReservedSpaceLabel" runat="server" Text="Reserved Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="ReservedSpace" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ReservedSpaceRequiredValidator" runat="server" ControlToValidate="ReservedSpace"
                    Display="Dynamic" ErrorMessage="&lt;br&gt;This field is required"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="SpaceValidator" runat="server" Display="Dynamic" ErrorMessage="&lt;br&gt;Inconsistent disk space values specified"
                    OnServerValidate="SpaceValidator_ServerValidate"></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ReadBandwidthLabel" runat="server" Text="Read Bandwidth:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="ReadBandwidth" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
                &nbsp;MB/sec<asp:RequiredFieldValidator ID="ReadBandwidthRequiredValidator" runat="server"
                    ControlToValidate="ReadBandwidth" Display="Dynamic" ErrorMessage="&lt;br&gt;This field is required"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="ReadBandwidthRangeValidator" runat="server" ControlToValidate="ReadBandwidth"
                    Display="Dynamic" ErrorMessage="&lt;br&gt;Invalid number" MaximumValue="100000"
                    MinimumValue="0" Type="Double"></asp:RangeValidator>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="WriteBandwidthLabel" runat="server" Text="Write Bandwidth:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="WriteBandwidth" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
                &nbsp;MB/sec<asp:RequiredFieldValidator ID="WriteBandwidthRequiredValidator" runat="server"
                    ControlToValidate="WriteBandwidth" Display="Dynamic" ErrorMessage="&lt;br&gt;This field is required"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="WriteBandwidthRangeValidator" runat="server" ControlToValidate="WriteBandwidth"
                    Display="Dynamic" ErrorMessage="&lt;br&gt;Invalid number" MaximumValue="100000"
                    MinimumValue="0" Type="Double"></asp:RangeValidator>
            </td>
        </tr>
    </table>
</asp:Content>
