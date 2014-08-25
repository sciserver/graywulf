<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.FederationForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="FederationForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm" runat="server">
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="ShortTitleLabel" runat="server" Text="Short Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="ShortTitle" runat="server" />
            </td>
        </tr>
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="LongTitleLabel" runat="server" Text="Long Title:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="LongTitle" runat="server" />
            </td>
        </tr>
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="EmailLabel" runat="server" Text="E-mail Address:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Email" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CopyrightLabel" runat="server" Text="Copyright:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Copyright" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DisclaimerLabel" runat="server" Text="Disclaimer:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Disclaimer" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SchemaManagerLabel" runat="server" Text="Schema Manager:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="SchemaManager" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="UserDatabaseFactoryLabel" runat="server" Text="User database factory:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="UserDatabaseFactory" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="QueryFactoryLabel" runat="server" Text="Query Factory:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="QueryFactory" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="StreamFactoryLabel" runat="server" Text="Stream Factory:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="StreamFactory" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FileFormatFactoryLabel" runat="server" Text="File Format Factory:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="FileFormatFactory" runat="server" />
            </td>
        </tr>
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="ControllerMachineLabel" runat="server" Text="Controller Machine:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntitySelect runat="server" id="ControllerMachine" CssClass="FormField" />
            </td>
        </tr>
        <tr runat="server">
            <td class="FormLabel">
                <asp:Label ID="SchemaSourceServerInstanceLabel" runat="server" Text="Schema Source Server:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntitySelect id="SchemaSourceServerInstance" runat="server" CssClass="FormField" />
            </td>
        </tr>
        <tr runat="server" id="MyDbServerVersionRow" visible="false">
            <td class="FormLabel">
                <asp:Label ID="MyDbServerVersionLabel" runat="server" Text="MYDB Server Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntitySelect ID="MyDbServerVersion" runat="server" CssClass="FormField" />
            </td>
        </tr>
        <tr runat="server" id="MyDbDatabaseVersionRow" visible="false">
            <td class="FormLabel">
                <asp:Label ID="MyDbDatabaseVersionLabel" runat="server" Text="MYDB Database Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntitySelect ID="MyDbDatabaseVersion" runat="server" CssClass="FormField" />
            </td>
        </tr>
        <tr runat="server" id="NodeServerVersionRow" visible="false">
            <td class="FormLabel">
                <asp:Label ID="NodeServerVersionLabel" runat="server" Text="Worker Node Server Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntitySelect ID="NodeServerVersion" runat="server" CssClass="FormField" />
            </td>
        </tr>
        <tr runat="server" id="TempDatabaseVersionRow" visible="false">
            <td class="FormLabel">
                <asp:Label ID="TempDatabaseVersionLabel" runat="server" Text="Temp Database Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntitySelect ID="TempDatabaseVersion" runat="server" CssClass="FormField" />
            </td>
        </tr>
        <tr runat="server" id="CodeDatabaseVersionRow" visible="false">
            <td class="FormLabel">
                <asp:Label ID="CodeDatabaseVersionLabel" runat="server" Text="Code Database Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntitySelect ID="CodeDatabaseVersion" runat="server" CssClass="FormField" />
            </td>
        </tr>
    </table>
</asp:Content>