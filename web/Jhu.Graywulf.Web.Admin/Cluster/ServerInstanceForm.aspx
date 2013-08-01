<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.ServerInstanceForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="ServerInstanceForm.aspx.cs" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ServerVersionLabel" runat="server" Text="Server Version:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="ServerVersion" runat="server" CssClass="FormField" OnSelectedIndexChanged="ServerVersion_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="InstanceNameLabel" runat="server" Text="Instance Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="InstanceName" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AdminAccessLabel" runat="server" Text="Admin Access:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:CheckBox ID="IntegratedSecurity" runat="server" AutoPostBack="True" Checked="True"
                    CssClass="FormField" OnCheckedChanged="IntegratedSecurity_CheckedChanged" Text="Integrated Security" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AdminUserLabel" runat="server" Text="Username:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="AdminUser" runat="server" CssClass="FormFieldNarrow" Enabled="False"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="AdminPasswordLabel" runat="server" Text="Password:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="AdminPassword" runat="server" CssClass="FormFieldNarrow" Enabled="False"
                    TextMode="Password"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
