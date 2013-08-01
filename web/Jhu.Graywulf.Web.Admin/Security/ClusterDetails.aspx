<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Security.ClusterDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="ClusterDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
</asp:Content>
<asp:Content ID="Content5" runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="DomainList" ChildrenType="Domain" EntityGroup="Security" />
        <jgwac:EntityList runat="server" ID="UserList" ChildrenType="User" EntityGroup="Security" Text="Cluster Users" />
        <jgwac:EntityList runat="server" ID="UserGroupList" ChildrenType="UserGroup" EntityGroup="Security" Text="Cluster User Groups" />
    </jgwac:EntityChildren>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
