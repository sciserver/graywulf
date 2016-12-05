<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportDetails.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.ExportDetails" %>

<div runat="server" class="gw-list-row gw-details-panel" style="display: none;">
    <span style="width: 100px; text-align: center"><asp:HyperLink runat="server" ID="cancel" Text="cancel" /></span><!--
 --><span style="width: 32px;"></span><!--
 --><asp:Label runat="server" Text="Source:" Width="128px" /><!--
 --><asp:Label runat="server" ID="source" CssClass="gw-list-span" />
</div>
<div runat="server" class="gw-list-row gw-details-panel" style="display: none;">
    <span style="width: 132px;"></span><!--
 --><asp:Label runat="server" Text="Destination:" Width="128px" /><!--
 --><asp:Label runat="server" ID="destination" CssClass="gw-list-span" />
</div>
<div runat="server" class="gw-list-row gw-details-panel" style="display: none;">
    <span style="width: 132px;"></span><!--
 --><asp:Label runat="server" Text="File format:" Width="128px" /><!--
 --><asp:Label runat="server" ID="fileFormat" CssClass="gw-list-span" />
</div>
