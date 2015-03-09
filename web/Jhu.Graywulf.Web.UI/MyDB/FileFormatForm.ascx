<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileFormatForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.MyDB.FileFormatForm" %>

<p>
    To override automatic file format detection, select format from the list below:
</p>
<table runat="server" class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="fileFormatListLabel">File format:</asp:Label>
        </td>
        <td class="FormField">
            <asp:DropDownList runat="server" ID="fileFormatList" CssClass="FormField" AutoPostBack="True" OnSelectedIndexChanged="FileFormat_SelectedIndexChanged" />
        </td>
    </tr>
</table>