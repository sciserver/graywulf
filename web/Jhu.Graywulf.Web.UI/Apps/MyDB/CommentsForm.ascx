<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentsForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.CommentsForm" %>

<table runat="server" class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="commentsLabel" Text="Job comments:" />
        </td>
        <td class="FormField">
            <asp:TextBox runat="server" ID="comments" CssClass="FormField" TextMode="MultiLine" />
        </td>
    </tr>
</table>
