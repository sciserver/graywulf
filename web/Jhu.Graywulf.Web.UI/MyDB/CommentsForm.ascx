<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentsForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.MyDB.CommentsForm" %>

<p>You can add some comments to the current import job:</p>
<table runat="server" class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="commentsLabel" Text="Comments:" />
        </td>
        <td class="FormField">
            <asp:TextBox runat="server" ID="comments" CssClass="FormField" TextMode="MultiLine" />
        </td>
    </tr>
</table>
