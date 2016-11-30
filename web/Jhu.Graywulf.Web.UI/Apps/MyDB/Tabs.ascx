<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tabs.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Tabs" %>
<jgwc:TabHeader ID="TabHeader" runat="server">
    <tabs>
        <jgwc:Tab runat="server" Text="Summary" ID="Summary" />
        <jgwc:Tab runat="server" Text="Tables" ID="Tables" />
        <jgwc:Tab runat="server" Text="Copy" ID="Copy" />
        <jgwc:Tab runat="server" Text="Import" ID="Import" />
        <jgwc:Tab runat="server" Text="Export" ID="Export" />
    </tabs>
</jgwc:TabHeader>
