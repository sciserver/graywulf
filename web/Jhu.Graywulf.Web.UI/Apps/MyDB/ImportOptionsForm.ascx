<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportOptionsForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.ImportOptionsForm" %>

<table runat="server" class="FormTable">
    <tr>
        <td class="FormLabel"></td>
        <td class="FormField">
            <asp:CheckBox runat="server" ID="generateIdentityColumn" Checked="true" Text="Automatically generate ID column" />
        </td>
    </tr>
</table>
