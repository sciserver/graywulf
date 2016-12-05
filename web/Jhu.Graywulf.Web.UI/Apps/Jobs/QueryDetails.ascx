<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QueryDetails.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.QueryDetails" %>

<div runat="server" class="gw-list-row gw-details-panel" style="display: none;">
    <span style="width: 100px; text-align: center">
        <asp:HyperLink runat="server" ID="cancel" Text="cancel" />
    </span>
    <span style="width: 32px;"></span>
    <asp:Label runat="server" Text="Source dataset:" />
    <asp:Label runat="server" ID="sourceDataset" />
    <asp:Label runat="server" Text="Source table:" />
    <asp:Label runat="server" ID="sourceTable" />
    <asp:Label runat="server" Text="Destination dataset:" />
    <asp:Label runat="server" ID="destinationDataset" />
    <asp:Label runat="server" Text="Destination table:" />
    <asp:Label runat="server" ID="destinationTable" />
</div>
