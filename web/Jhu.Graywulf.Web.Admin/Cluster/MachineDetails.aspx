<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.MachineDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="MachineDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm dock-top">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="HostNameLabel" runat="server" Text="Host Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="HostName" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AdminUrlLabel" runat="server" Text="Admin URL:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:HyperLink ID="AdminUrl" runat="server" Target="_blank">HyperLink</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DeployUncPathLabel" runat="server" Text="Deployment Path:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:HyperLink ID="DeployUncPath" runat="server" Target="_blank">HyperLink</asp:HyperLink>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" RunningStateButtonsVisible="true" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren ID="EntityChildren1" runat="server">
        <jgwac:EntityList runat="server" ID="DiskVolumeList" ChildrenType="DiskVolume" EntityGroup="Cluster">
            <columns>
                        <asp:BoundField DataField="DiskVolumeType" HeaderText="Type" />
                        <asp:BoundField DataField="FullSpace" HeaderText="Full Space" />
                        <jgwc:ExpressionPropertyField DataField="LocalPath" HeaderText="Local Path" />
                        <jgwc:ExpressionPropertyField DataField="UncPath" HeaderText="UNC Path" />
                    </columns>
        </jgwac:EntityList>
        <jgwac:EntityList runat="server" ID="ServerInstanceList" ChildrenType="ServerInstance"
            EntityGroup="Cluster" />
    </jgwac:EntityChildren>
</asp:Content>
