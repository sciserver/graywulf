<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.DiskGroupDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="DiskGroupDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm dock-top">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TypeLabel" runat="server" Text="Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Type" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FullSpaceLabel" runat="server" Text="Full Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="FullSpace" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AllocatedSpaceLabel" runat="server" Text="Allocated Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="AllocatedSpace" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ReservedSpaceLabel" runat="server" Text="Reserved Space:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ReservedSpace" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="UsageLabel" runat="server" Text="Disk Volume Usage:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwc:UsageBar ID="Usage" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ReadBandwidthLabel" runat="server" Text="Read Bandwidth:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ReadBandwidth" runat="server" Text="Label"></asp:Label>
                &nbsp;MB/sec
            </td>
        </tr>
         <tr>
            <td class="FormLabel">
                <asp:Label ID="WriteBandwidthLabel" runat="server" Text="Write Bandwidth:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="WriteBandwidth" runat="server" Text="Label"></asp:Label>
                &nbsp;MB/sec
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" RunningStateButtonsVisible="true" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server" ID="EntityChildren">
        <jgwac:EntityList runat="server" ID="DiskVolumeList" ChildrenType="DiskVolume" EntityGroup="Cluster">
            <columns>
                <asp:BoundField DataField="Type" HeaderText="Type" />
                <asp:BoundField DataField="FullSpace" HeaderText="Full Space" />
                <jgwc:ExpressionPropertyField DataField="LocalPath" HeaderText="Local Path" />
                <jgwc:ExpressionPropertyField DataField="UncPath" HeaderText="UNC Path" />
            </columns>
        </jgwac:EntityList>
    </jgwac:EntityChildren>
</asp:Content>
