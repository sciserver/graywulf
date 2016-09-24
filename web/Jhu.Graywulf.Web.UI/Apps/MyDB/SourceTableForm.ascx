<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SourceTableForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.SourceTableForm" %>

<table class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="tableListLabel">Table name:</asp:Label>
        </td>
        <td class="FormField">
            <asp:DropDownList runat="server" ID="tableList" CssClass="FormField" />
        </td>
    </tr>
</table>
