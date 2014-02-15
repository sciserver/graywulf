<%@ Control Language="C#" 
    Inherits="Jhu.Graywulf.Web.Admin.Controls.EntitySettings" Codebehind="EntitySettings.ascx.cs" %>
<%@ Register Src="EntityLink.ascx" TagName="EntityLink" TagPrefix="jgw" %>

<%-- <asp:TextBox runat="server" ID="Settings" Rows="10" CssClass="Settings dock-fill" TextMode="MultiLine" /> --%>
<jgwc:CodeMirror runat="server" ID="Settings" Mode="text/xml" Theme="default" CssClass="dock-fill" Width="100%" Height="100%" />
