<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.ClusterDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="ClusterDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="DomainList" ChildrenType="Domain" EntityGroup="Jobs" />
        <jgwac:EntityList runat="server" ID="MachineRoleList" ChildrenType="MachineRole"
            EntityGroup="Jobs" />
        <jgwac:EntityList runat="server" ID="QueueDefinitionList" ChildrenType="QueueDefinition"
            EntityGroup="Jobs" />
    </jgwac:EntityChildren>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
