<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.MachineRoleDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="MachineRoleDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm dock-top">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="MachineRoleTypeLabel" runat="server" Text="Machine Role Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="MachineRoleType" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="MachineList" ChildrenType="Machine" EntityGroup="Cluster">
            <Columns>
                <jgwc:ExpressionPropertyField DataField="HostName" HeaderText="UNC name" />
                <asp:BoundField DataField="RunningState" HeaderText="Running state" />
            </Columns>
        </jgwac:EntityList>
        <jgwac:EntityList runat="server" ID="ServerVersionList" ChildrenType="ServerVersion"
            EntityGroup="Cluster" />
    </jgwac:EntityChildren>
</asp:Content>
