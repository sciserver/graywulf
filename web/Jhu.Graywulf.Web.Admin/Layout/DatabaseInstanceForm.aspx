<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.DatabaseInstanceForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="DatabaseInstanceForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SliceLabel" runat="server" Text="Slice:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="Slice" runat="server" CssClass="FormField">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseVersionLabel" runat="server" Text="Database Version:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="DatabaseVersion" runat="server" CssClass="FormField" 
                    AutoPostBack="True" 
                    onselectedindexchanged="DatabaseVersion_SelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ServerInstanceLabel" runat="server" Text="Server Instance:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="ServerInstance" runat="server" CssClass="FormField" AutoPostBack="true"
                    OnSelectedIndexChanged="ServerInstance_SelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseNameLabel" runat="server" Text="Database Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="DatabaseName" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>