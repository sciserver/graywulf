<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.DatabaseVersionDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="DatabaseVersionDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ServerVersionLabel" runat="server" Text="Server Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="ServerVersion" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren ID="EntityChildren1" runat="server">
        <jgwac:EntityList runat="server" ID="UserDatabaseInstanceList" ChildrenType="UserDatabaseInstance" EntityGroup="Layout" />
    </jgwac:EntityChildren>
</asp:Content>