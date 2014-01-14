<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Domain.DomainDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="DomainDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
</asp:Content>
<asp:Content ID="Content5" runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="UserList" ChildrenType="User" EntityGroup="Domain" Text="Domain Users" />
        <jgwac:EntityList runat="server" ID="UserGroupList" ChildrenType="UserGroup" EntityGroup="Domain" Text="Domain User Groups" />
    </jgwac:EntityChildren>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
