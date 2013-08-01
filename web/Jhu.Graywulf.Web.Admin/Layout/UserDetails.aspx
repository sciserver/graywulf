<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.UserDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="UserDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="FirstName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="MiddleNameLable" runat="server" Text="Middle Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="MiddleName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="LastName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="EmailLabel" runat="server" Text="E-mail Address:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Email" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="CompanyLabel" runat="server" Text="Company:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Company" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="IntegratedLabel" runat="server" Text="Integrated:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Integrated" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="NtlmUserLabel" runat="server" Text="Windows User Account:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="NtlmUser" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="UserDatabaseInstanceList" ChildrenType="UserDatabaseInstance" EntityGroup="Layout" />
    </jgwac:EntityChildren>
</asp:Content>
