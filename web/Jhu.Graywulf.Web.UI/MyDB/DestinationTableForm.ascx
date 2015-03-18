<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DestinationTableForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.MyDB.DestinationTableForm" %>

<p>To override automatic table name generation, specify table name below:</p>
<table runat="server" class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="tableNameLabel">Table name:</asp:Label>
        </td>
        <td class="FormField">
            <asp:TextBox ID="tableName" runat="server" CssClass="FormField"></asp:TextBox>
            <asp:CustomValidator runat="server" ControlToValidate="tableName" Display="Dynamic"
                ErrorMessage="<br />Invalid table name" ID="tableNameValidator"
                OnServerValidate="TableNameValidator_ServerValidate" />
        </td>
    </tr>
</table>
