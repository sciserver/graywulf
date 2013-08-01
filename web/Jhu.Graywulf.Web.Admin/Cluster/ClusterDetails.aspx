<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.ClusterDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="ClusterDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="MachineRoleList" ChildrenType="MachineRole"
            EntityGroup="Cluster" />
    </jgwac:EntityChildren>
</asp:Content>
