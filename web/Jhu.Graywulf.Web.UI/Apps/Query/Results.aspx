<%@ Page Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Query.Results" MasterPageFile="~/App_Masters/Basic/UI.master" CodeBehind="Results.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <asp:Panel runat="server" ID="toolbarPanel" class="ToolbarFrame">
        <div class="Toolbar">
            <div class="ToolbarElement" style="width: 200px">
                <asp:Label ID="fileFormatLabel" runat="server" Text="File format:" /><br />
                <asp:DropDownList ID="fileFormat" runat="server" CssClass="ToolbarControl" Style="width: 100%;"></asp:DropDownList>
            </div>
            <asp:LinkButton ID="download" runat="server" Text="download" CssClass="ToolbarButton" OnClick="Download_Click" />
            <div class="ToolbarElement"  style="width: auto"></div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <asp:Panel runat="server" ID="resultsPanel" Visible="true" CssClass="dock-fill dock-scroll">
        <% RenderResults(); %>
    </asp:Panel>
</asp:Content>
