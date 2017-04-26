<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.Admin.Controls.Menu" Codebehind="Menu.ascx.cs" %>

<jgwc:Toolbar runat="server" ID="TheMenu" SkinID="Menu">
    <jgwc:ToolbarButton ID="Home" runat="server" Text="home" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Search" runat="server" Text="search" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Cluster" runat="server" Text="cluster" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Domain" runat="server" Text="domain" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Federation" runat="server" Text="federation" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Layout" runat="server" Text="layout" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Jobs" runat="server" Text="jobs" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Monitor" runat="server" Text="monitor" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Log" runat="server" Text="log" SkinID="Menu" />
    <jgwc:ToolbarButton ID="Docs" runat="server" Text="docs" SkinID="Menu" />
    <jgwc:ToolbarSpan runat="server" SkinID="Menu" />
</jgwc:Toolbar>