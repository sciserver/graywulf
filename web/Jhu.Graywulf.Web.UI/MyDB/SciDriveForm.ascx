<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SciDriveForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.MyDB.SciDriveForm" %>

<table class="FormTable">
    <tr>
        <td class="FormLabel" style="width: 50px">
            <asp:Label runat="server" ID="uriLabel">Path:</asp:Label>&nbsp;&nbsp;
        </td>
        <td class="FormField" style="width: 420px">
            <asp:TextBox runat="server" ID="uri" CssClass="FormField" Width="420px" />
        </td>
    </tr>
</table>
