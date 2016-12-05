<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QueryDetails.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.QueryDetails" %>

<div runat="server" class="gw-list-row gw-details-panel" style="display: none;">
    <span style="width: 100px; text-align: center"><asp:HyperLink runat="server" ID="cancel" Text="cancel" /></span><!--
 --><span style="width: 32px;"></span><!--
 --><asp:Label runat="server" Text="Output:" Width="128px" /><!--
 --><asp:Label runat="server" ID="output" CssClass="gw-list-span" />
</div>
<div runat="server" class="gw-list-row gw-details-panel" style="display: none;">
    <span style="width: 100px; text-align: center"><asp:HyperLink runat="server" ID="edit" Text="edit query" /></span><!--
 --><span style="width: 32px;"></span><!--
 --><asp:Label runat="server" Text="Query:" Width="128px" /><!--
 --><asp:Label runat="server" ID="query" CssClass="gw-list-span" />
</div>