<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DestinationTableForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.DestinationTableForm" %>

<%@ Register Src="~/Apps/MyDB/DatasetList.ascx" TagPrefix="jgwu" TagName="DatasetList" %>

<table runat="server" class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="datasetListLabel">Destination dataset:</asp:Label>
        </td>
        <td class="FormField">
            <jgwu:DatasetList runat="server" id="datasetList" CssClass="FormField" />
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="tableNameLabel">Destination table:</asp:Label>
        </td>
        <td class="FormField">
            <asp:TextBox ID="tableName" runat="server" CssClass="FormField" />
            <asp:CustomValidator runat="server" ControlToValidate="tableName" Display="Dynamic"
                ErrorMessage="<br />Invalid table name" ID="tableNameValidator"
                OnServerValidate="TableNameValidator_ServerValidate" />
        </td>
    </tr>
</table>
