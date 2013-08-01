<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.JobDefinitionDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="JobDefinitionDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TypeNameLabel" runat="server" Text="Type Name:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="TypeName" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <div class="dock-top">
        <jgwc:MultiViewTabHeader ID="Tabs" runat="server" MultiViewID="MultiViewTabs" />
    </div>
    <div class="TabFrame dock-fill">
        <asp:MultiView runat="server" ID="MultiViewTabs" ActiveViewIndex="0">
            <jgwc:TabView runat="server" Text="Parameters">
                <asp:BulletedList ID="Parameters" runat="server">
                </asp:BulletedList>
            </jgwc:TabView>
            <jgwc:TabView runat="server" Text="Checkpoints">
                <jgwac:CheckpointProgress ID="CheckpointProgress" runat="server" />
            </jgwc:TabView>
        </asp:MultiView>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
    |
    <asp:Button ID="Discover" runat="server" CssClass="FormButtonNarrow" Text="Discover"
        OnCommand="Button_Command" CommandName="Discover" />
</asp:Content>
