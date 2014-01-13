<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.DatabaseDefinitionDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="DatabaseDefinitionDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
    |
    <asp:Button ID="Map" runat="server" CssClass="FormButtonNarrow" Text="Map" OnCommand="Button_Command"
        CommandName="Map" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="DatabaseInstanceList" ChildrenType="DatabaseInstance"
            EntityGroup="Layout">
            <columns>
                <asp:BoundField DataField="DeploymentState" HeaderText="Deployment State" />
                <asp:BoundField DataField="RunningState" HeaderText="Running State" />
            </columns>
        </jgwac:EntityList>
        <jgwac:EntityList runat="server" ID="DatabaseVersionList" ChildrenType="DatabaseVersion" EntityGroup="Layout" />
    </jgwac:EntityChildren>
</asp:Content>
