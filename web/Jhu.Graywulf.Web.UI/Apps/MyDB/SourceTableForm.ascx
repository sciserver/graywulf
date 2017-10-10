<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SourceTableForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.SourceTableForm" %>

<%@ Register Src="~/Apps/MyDB/DatasetList.ascx" TagPrefix="jgwu" TagName="DatasetList" %>
<%@ Register Src="~/Apps/MyDB/TableList.ascx" TagPrefix="jgwu" TagName="TableList" %>


<table class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="datasetListLabel">Source dataset:</asp:Label>
        </td>
        <td class="FormField">
            <jgwu:DatasetList runat="server" id="datasetList" CssClass="FormField"
                AutoPostBack="true" TableListControl="tableList" DefaultRequestField="objid"  />
        </td>
    </tr>
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="tableListLabel">Source table:</asp:Label>
        </td>
        <td class="FormField">
            <jgwu:TableList runat="server" id="tableList" CssClass="FormField"  DefaultRequestField="objid"
                OnSelectedTableChanged="TableList_SelectedTableChanged" AutoPostBack="true" />
        </td>
    </tr>
</table>
