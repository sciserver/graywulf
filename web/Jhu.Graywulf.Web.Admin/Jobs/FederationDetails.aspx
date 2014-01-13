<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.FederationDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="FederationDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
</asp:Content>
<asp:Content ID="Content5" runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="JobDefinitionList" ChildrenType="JobDefinition"
            EntityGroup="Jobs" />
    </jgwac:EntityChildren>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
