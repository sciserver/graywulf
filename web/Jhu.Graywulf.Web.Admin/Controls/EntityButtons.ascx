<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntityButtons.ascx.cs"
    Inherits="Jhu.Graywulf.Web.Admin.Controls.EntityButtons" %>
<span runat="server" id="BasicButtons">
    <asp:Button ID="Edit" runat="server" CssClass="FormButtonNarrow" Text="Edit" OnCommand="Button_Command"
        CommandName="Edit" />
    <asp:Button ID="Delete" runat="server" CssClass="FormButtonNarrow" Text="Delete"
        OnCommand="Button_Command" CommandName="Delete" />
    <asp:Button ID="Serialize" runat="server" CssClass="FormButtonNarrow" OnCommand="Button_Command"
        CommandName="Serialize" Text="Serialize" />
    |
    <asp:Button ID="ToggleShowHide" runat="server" CssClass="FormButtonNarrow" Text="Show"
        OnCommand="Button_Command" CommandName="ToggleShowHide" />
</span>|
<asp:Button ID="ToggleDeploymentState" runat="server" CssClass="FormButtonNarrow"
    Text="Deploy" OnCommand="Button_Command" CommandName="ToggleDeploymentState" />
<asp:Button ID="ToggleRunningState" runat="server" CssClass="FormButtonNarrow" Text="Start"
    OnCommand="Button_Command" CommandName="ToggleRunningState" />
<span runat="server" id="DiscoverButton">|
    <asp:Button ID="Discover" runat="server" CssClass="FormButtonNarrow" OnCommand="Button_Command"
        CommandName="Discover" Text="Discover" />
</span>