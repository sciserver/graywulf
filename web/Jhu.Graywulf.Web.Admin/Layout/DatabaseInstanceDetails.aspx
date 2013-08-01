<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.DatabaseInstanceDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="DatabaseInstanceDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseNameLabel" runat="server" Text="Database Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="DatabaseName" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ServerInstanceLabel" runat="server" Text="Server Instance:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="ServerInstance" Expression="[$Machine.Name].[$Name]" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="SliceLabel" runat="server" Text="Slice:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="Slice" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DatabaseVersionLabel" runat="server" Text="Database Version:"></asp:Label>
            </td>
            <td class="FormField">
                <jgwac:EntityLink ID="DatabaseVersion" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" DiscoverButtonVisible="true" />
    |
    <asp:Button ID="Allocate" runat="server" CssClass="FormButtonNarrow" Text="Allocate"
        OnCommand="Button_Command" CommandName="Allocate"  />
    <asp:Button ID="Drop" runat="server" CssClass="FormButtonNarrow" Text="Drop"
        OnCommand="Button_Command" CommandName="Drop" />
    <asp:Button ID="Attach" runat="server" CssClass="FormButtonNarrow" Text="Attach"
        OnCommand="Button_Command" CommandName="Attach" />
    <asp:Button ID="Detach" runat="server" CssClass="FormButtonNarrow" Text="Detach"
        OnCommand="Button_Command" CommandName="Detach" />
</asp:Content>
<asp:Content ID="Content5" runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="FileGroupList" ChildrenType="DatabaseInstanceFileGroup"
            EntityGroup="Layout" />
    </jgwac:EntityChildren>
</asp:Content>

