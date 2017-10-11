<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileFormatForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.FileFormatForm" %>

<table runat="server" class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="fileFormatListLabel">File format:</asp:Label>
        </td>
        <td class="FormField">
            <asp:DropDownList runat="server" ID="fileFormatList" CssClass="FormField" OnSelectedIndexChanged="FileFormatList_SelectedIndexChanged" />
            <asp:RequiredFieldValidator runat="server" ID="fileFormatListRequiredValidator" Display="Dynamic" ControlToValidate="fileFormatList"
                ErrorMessage="<br />File format must be selected." EnableClientScript="false" />
        </td>
    </tr>
</table>
