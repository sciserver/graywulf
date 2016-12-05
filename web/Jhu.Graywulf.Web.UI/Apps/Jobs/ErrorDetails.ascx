<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ErrorDetails.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.ErrorDetails" %>

<div runat="server" class="gw-list-row gw-details-panel" style="display: none;">
    <span style="width: 132px;"></span>
    <span runat="server" id="errorLabel" style="width: 128px; text-align: left">Error:</span>
    <asp:Label runat="server" ID="error" CssClass="gw-list-span" />
</div>
<div runat="server" class="gw-list-row gw-details-panel" style="display: none;">
    <span style="width: 132px;"></span>
    <span class="gw-list-span">
        If the error is permanent and you think it is a bug, 
        <asp:HyperLink runat="server" ID="errorInquiry" Text="send an inquiry" />.
    </span>
</div>