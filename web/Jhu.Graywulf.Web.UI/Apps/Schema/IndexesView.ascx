<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.IndexesView" Codebehind="IndexesView.ascx.cs" %>

<div class="dock-top gw-list-frame-top">
    <div class="gw-list-header">
        <div class="gw-list-row">
            <!--<span style="width: 32px"></span>-->
            <span style="width: 180px">index name</span>
            <span style="width: 300px">index columns</span>
            <span class="gw-list-span">included columns</span>
        </div>
    </div>
</div>
<div class="dock-bottom gw-list-frame-bottom">
</div>
<div class="gw-list-frame dock-fill">
    <jgwc:MultiSelectListView runat="server" ID="listView">
        <LayoutTemplate>
            <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
        </LayoutTemplate>
        <ItemTemplate>
            <div class="gw-list-item gw-details-container">
                <div class="gw-list-row">
                    <!--<jgwc:DetailsButton runat="server" Style="width: 32px" />-->
                    <asp:Label runat="server" Text='<%# Eval("IndexName") %>' Width="180px" />
                    <asp:Label runat="server" Text='<%# Eval("ColumnListDisplayString") %>' Width="300px" />
                    <asp:Label runat="server" Text='<%# Eval("IncludedColumnListDisplayString") %>' CssClass="gw-list-multiline-span" />
                </div>
            </div>
        </ItemTemplate>
    </jgwc:MultiSelectListView>
</div>
