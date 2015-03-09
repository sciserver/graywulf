<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CredentialsForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.MyDB.CredentialsForm" %>

<p>
    To fetch data from a remote source you may need to specify credentials:
</p>
<table runat="server" class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="usernameLabel">User name:</asp:Label>
        </td>
        <td class="FormField">
            <asp:TextBox ID="username" runat="server" CssClass="FormField" />
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="passwordLabel">Password:</asp:Label>
        </td>
        <td class="FormField">
            <asp:TextBox ID="password" runat="server" CssClass="FormField" TextMode="Password" />
        </td>
    </tr>
</table>
