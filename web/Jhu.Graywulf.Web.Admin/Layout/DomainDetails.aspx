<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.DomainDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="DomainDetails.aspx.cs" %>

<asp:Content ID="Content5" runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren ID="EntityChildren1" runat="server">
        <jgwac:EntityList runat="server" ID="FederationList" ChildrenType="Federation" EntityGroup="Layout" />
        <jgwac:EntityList runat="server" ID="UserList" ChildrenType="User" EntityGroup="Layout" />
    </jgwac:EntityChildren>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
