<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportRow.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.ExportRow" %>
<tr>
    <td style="width: 32px">
        <asp:CheckBox runat="server" ID="selection" /></td>
    <td style="width: 32px">
        <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span></td>
    <td style="width: 100px">
        <jgwuc:JobStatus runat="server" Status='<%# Eval("Status") %>' Width="100px" Style="text-align: center" /></td>
    <td style="width: 100px; text-align: left">
        <asp:Label runat="server" Text='<%# Eval("Type") %>' /></td>
    <td style="width: 120px">
        <jgwc:FancyDateLabel runat="server" Value='<%# Eval("DateCreated") %>' /></td>
    <td style="width: 120px">
        <jgwc:FancyDateLabel runat="server" Value='<%# Eval("DateStarted") %>' /></td>
    <td style="width: 120px">
        <jgwc:FancyDateLabel runat="server" Value='<%# Eval("DateFinished") %>' /></td>
    <td style="width: auto; text-align: left">
        <asp:Label runat="server" Text='<%# Eval("Comments") %>' /></td>
    </td>
</tr>
