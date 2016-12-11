<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Toolbar.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Docs.Toolbar" %>

<div runat="server" id="toolbar" class="toolbar">
    <asp:PlaceHolder runat="server" ID="buttonPlaceHolder" />
    <div class="span"></div>
    <asp:HyperLink runat="server" ID="prevButton" Text="prev page" />
    <asp:HyperLink runat="server" ID="nextButton" Text="next page"/>
    <asp:HyperLink runat="server" ID="upButton" Text="up"/>
</div>