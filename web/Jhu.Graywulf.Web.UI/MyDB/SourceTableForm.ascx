<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SourceTableForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.MyDB.SourceTableForm" %>

<table class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="TableNameLable">Table name:</asp:Label>
        </td>
        <td class="FormField">
            <asp:DropDownList runat="server" ID="TableName" CssClass="FormField" />
        </td>
    </tr>
</table>
