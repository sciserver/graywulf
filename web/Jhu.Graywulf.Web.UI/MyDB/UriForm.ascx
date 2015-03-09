<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UriForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.MyDB.UriForm" %>

<table class="FormTable">
    <tr>
        <td class="FormLabel" style="width: 50px">
            <asp:Label runat="server" ID="uriLabel">URI:</asp:Label>&nbsp;&nbsp;
        </td>
        <td class="FormField" style="width: 420px">
            <asp:TextBox runat="server" ID="uri" CssClass="FormField" Width="420px" />
        </td>
    </tr>
</table>
