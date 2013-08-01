<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.DatabaseVersionForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="DatabaseVersionForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ServerVersionLabel" runat="server" Text="Server Version:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="ServerVersion" runat="server" CssClass="FormField">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SizeMultiplierLabel" runat="server" Text="Size Multiplier:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="SizeMultiplier" runat="server" CssClass="FormFieldNarrow" />
            </td>
        </tr>
    </table>
</asp:Content>