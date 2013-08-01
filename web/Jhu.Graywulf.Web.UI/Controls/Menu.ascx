<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Controls.Menu" Codebehind="Menu.ascx.cs" %>

<jgwc:Toolbar runat="server" SkinID="Menu">
    <jgwc:ToolbarButton ID="Home" runat="server" Text="home" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Schema" runat="server" Text="schema" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Query" runat="server" Text="query" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Jobs" runat="server" Text="jobs" SkinID="Menu" />
    <jgwc:ToolbarButton ID="MyDB" runat="server" Text="my db" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Docs" runat="server" Text="docs" SkinID="Menu" />
    <jgwc:ToolbarSpan runat="server" SkinID="Menu" />
</jgwc:Toolbar>
