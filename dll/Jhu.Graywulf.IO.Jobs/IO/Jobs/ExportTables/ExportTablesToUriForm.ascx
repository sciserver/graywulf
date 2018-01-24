<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportTablesToUriForm.ascx.cs" Inherits="Jhu.Graywulf.IO.Jobs.ExportTables.ExportTablesToUriForm" %>

<p>
    Specify a URI to save the data file to:
</p>
<table class="FormTable">
    <tr>
        <td class="FormLabel" style="width: 50px">
            <asp:Label runat="server" ID="uriLabel">URI:</asp:Label>&nbsp;&nbsp;
        </td>
        <td class="FormField" style="width: 420px">
            <asp:TextBox runat="server" ID="uri" CssClass="FormField" Width="100%" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="uri" Display="Dynamic"
                ErrorMessage="<br />The URI field is required" />
            <asp:RegularExpressionValidator runat="server" ID="uriFormatValidator" ControlToValidate="uri" Display="Dynamic"
                ErrorMessage="<br />Invalid URI format" />
        </td>
    </tr>
</table>
<p>
    To save data to a remote server you may need to provide your credentials:
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
