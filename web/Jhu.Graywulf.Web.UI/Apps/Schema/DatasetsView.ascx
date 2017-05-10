<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.DatasetsView"
    CodeBehind="DatasetsView.ascx.cs" %>

<div class="dock-top gw-list-frame-top">
    <div class="gw-list-header">
        <div class="gw-list-row">
            <span style="width: 128px"></span>
            <span style="width: 180px">data set name</span>
            <span class="gw-list-span">summary</span>
        </div>
    </div>
</div>
<div class="dock-bottom gw-list-frame-bottom">
</div>
<div class="gw-list-frame dock-fill">
    <jgwc:MultiSelectListView runat="server" ID="listView" OnItemCreated="ListView_ItemCreated" OnItemCommand="ListView_ItemCommand">
        <LayoutTemplate>
            <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
        </LayoutTemplate>
        <ItemTemplate>
            <div class="gw-list-item gw-details-container">
                <div class="gw-list-row">
                    <asp:LinkButton runat="server" CommandName="dataset_click"><asp:Image runat="server" ID="icon" Width="120px" Height="60px" style="margin-right: 8px" /></asp:LinkButton>
                    <asp:Label runat="server" Width="180px" CssClass="gw-list-multiline-span">
                        <asp:LinkButton runat="server" CommandName="dataset_click"><%# Eval("Name") %></asp:LinkButton><br />
                        <%# Eval("Metadata.Summary") %>
                    </asp:Label>
                </div>
            </div>
        </ItemTemplate>
    </jgwc:MultiSelectListView>
</div>
