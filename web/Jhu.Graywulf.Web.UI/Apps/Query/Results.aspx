<%@ Page Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Query.Results" MasterPageFile="~/App_Masters/Basic/UI.master" CodeBehind="Results.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <div class="toolbar">
        <div style="min-width: 300px;">
            <asp:Label ID="fileFormatLabel" runat="server" Text="File format:" /><br />
            <asp:DropDownList ID="fileFormat" runat="server" CssClass="ToolbarControl" Style="width: 100%;"></asp:DropDownList>
        </div>
        <asp:LinkButton ID="download" runat="server" Text="download" OnClick="Download_Click" />
        <div class="span"></div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <asp:Panel runat="server" ID="resultsPanel" Visible="true" CssClass="dock-fill dock-scroll">
        <% RenderResults(); %>
    </asp:Panel>
</asp:Content>
