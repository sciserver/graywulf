<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.FederationDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="FederationDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ShortTitleLabel" runat="server" Text="Short Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ShortTitle" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="LongTitleLabel" runat="server" Text="Long Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="LongTitle" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="EmailLabel" runat="server" Text="Email:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Email" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="MyDbDatabaseVersionLabel" runat="server" Text="MYDB Database Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="MyDbDatabaseVersion" Expression="[$DatabaseDefinition.Name].[$Name]"
                    runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TempDatabaseVersionLabel" runat="server" Text="Temp Database Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="TempDatabaseVersion" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CodeDatabaseVersionLabel" runat="server" Text="Code Database Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="CodeDatabaseVersion" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ControllerMachineLabel" runat="server" Text="Controller Machine:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="ControllerMachine" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SchemaSourceServerInstanceLabel" runat="server" Text="Schema Source Server:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="SchemaSourceServerInstance" Expression="[$Machine.Name].[$Name]"
                    runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="QueryFactoryTypeNameLabel" runat="server" Text="Query Factory:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="QueryFactoryTypeName" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FileFormatFactoryTypeNameLabel" runat="server" Text="File Format Factory:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="FileFormatFactoryTypeName" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="DatabaseDefinitionList" ChildrenType="DatabaseDefinition" EntityGroup="Federation">
            <Columns>
                <asp:BoundField DataField="DeploymentState" HeaderText="Deployment State" />
                <asp:BoundField DataField="RunningState" HeaderText="Running State" />
            </Columns>
        </jgwac:EntityList>
        <jgwac:EntityList runat="server" ID="RemoteDatabaseList" ChildrenType="RemoteDatabase" EntityGroup="Federation">
            <Columns>
                <asp:BoundField DataField="DeploymentState" HeaderText="Deployment State" />
                <asp:BoundField DataField="RunningState" HeaderText="Running State" />
            </Columns>
        </jgwac:EntityList>
    </jgwac:EntityChildren>
</asp:Content>
