<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.SliceForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="SliceForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="FromLabel" runat="server" Text="From:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="From" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ToLabel" runat="server" Text="To:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="To" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
