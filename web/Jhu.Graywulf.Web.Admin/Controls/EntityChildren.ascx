<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntityChildren.ascx.cs"
    Inherits="Jhu.Graywulf.Web.Admin.Controls.EntityChildren" %>
<div class="dock-top">
    <jgwc:MultiViewTabHeader ID="Tabs" runat="server" MultiViewID="MultiViewTabs" />
</div>
<div class="TabFrame dock-fill dock-container">
    <asp:MultiView runat="server" ID="MultiViewTabs" ActiveViewIndex="0">
    </asp:MultiView>
</div>
