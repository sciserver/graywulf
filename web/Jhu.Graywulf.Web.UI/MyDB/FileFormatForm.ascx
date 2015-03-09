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
    <tr runat="server" id="generateIdentityRow">
        <td class="FormLabel">&nbsp;
        </td>
        <td class="FormField">
            <asp:CheckBox runat="server" ID="generateIdentity" CssClass="FormField" Text="Generate identity column"
                Checked="true" />
        </td>
    </tr>
</table>