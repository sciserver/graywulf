<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.DatabaseDefinitionDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="DatabaseDefinitionDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SchemaSourceServerInstanceLabel" runat="server" Text="Schema Source Server:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="SchemaSourceServerInstance" Expression="[$Machine.Name]\[$Name]"
                    runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SchemaSourceDatabaseNameLabel" runat="server" Text="Schema Source DB:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="SchemaSourceDatabaseName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="LayoutTypeLabel" runat="server" Text="Layout Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="LayoutType" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseInstanceNamePatternLabel" runat="server" Text="DB Instance Name Pattern:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="DatabaseInstanceNamePattern" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseNamePatternLabel" runat="server" Text="DB Name Pattern:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="DatabaseNamePattern" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SliceCountLabel" runat="server" Text="Slice Count:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="SliceCount" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="PartitionCountLabel" runat="server" Text="Partition Count:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="PartitionCount" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="PartitionRangeTypeLabel" runat="server" Text="Range Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="PartitionRangeType" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="PartitionFunctionLabel" runat="server" Text="Partition Function:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="PartitionFunction" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" DiscoverButtonVisible="true" RunningStateButtonsVisible="true" />
    |
    <asp:Button ID="Slice" runat="server" CssClass="FormButtonNarrow" Text="Slice" OnCommand="Button_Command"
        CommandName="Slice" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="FileGroupList" ChildrenType="FileGroup" EntityGroup="Federation">
            <columns>
                <asp:BoundField DataField="FileGroupType" HeaderText="Type" />
                <asp:BoundField DataField="LayoutType" HeaderText="Layout" />
                <jgwc:BoundByteSizeField DataField="AllocatedSpace" HeaderText="Size" />
            </columns>
        </jgwac:EntityList>
        <jgwac:EntityList runat="server" ID="SliceList" ChildrenType="Slice" EntityGroup="Federation " />
        <jgwac:EntityList runat="server" ID="DatabaseVersionList" ChildrenType="DatabaseVersion"
            EntityGroup="Federation" />
        <jgwac:EntityList runat="server" ID="DeploymentPackageList" ChildrenType="DeploymentPackage"
            EntityGroup="Federation" />
    </jgwac:EntityChildren>
</asp:Content>
