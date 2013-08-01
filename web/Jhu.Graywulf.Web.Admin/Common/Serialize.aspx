<%@ Page Language="C#"  Inherits="Jhu.Graywulf.Web.Admin.Common.Serialize"
    MasterPageFile="~/Admin.master" CodeBehind="Serialize.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <table class="Form" cellspacing="0" cellpadding="0">
        <tr>
            <td class="FormLabel">
            </td>
            <td class="FormField">
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
            </td>
            <td class="FormField">
            </td>
        </tr>
    </table>
    <p class="FormButton">
        <asp:Button ID="Ok" runat="server" Text="Ok" CssClass="FormButton" OnClick="Ok_Click" />
    </p>
</asp:Content>
