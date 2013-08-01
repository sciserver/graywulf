<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.FederationDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="FederationDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren ID="EntityChildren1" runat="server">
        <jgwac:EntityList runat="server" ID="DatabaseDefinitionList" ChildrenType="DatabaseDefinition"
            EntityGroup="Layout" />
    </jgwac:EntityChildren>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
