<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.ColumnList"
    CodeBehind="ColumnList.ascx.cs" %>

<div class="dock-top gw-list-frame-top">
    <div class="gw-list-header">
        <div class="gw-list-row">
            <!--<span style="width: 32px"></span>-->
            <span style="width: 180px">column name</span>
            <span style="width: 150px">data type</span>
            <span style="width: 64px">byte size</span>
            <span style="width: 150px">class</span>
            <span style="width: 150px">quantity</span>
            <span style="width: 150px">unit</span>
            <!--<span class="gw-list-span">comments</span>-->
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
                    <asp:Label runat="server" Text='<%# Eval("Name") %>' Width="180px" />
                    <asp:Label runat="server" Text='<%# Eval("DataType.TypeNameWithLength") %>' Width="150px" />
                    <asp:Label runat="server" Text='<%# Eval("DataType.ByteSize") %>' Width="64px" />
                    <asp:Label runat="server" Text='<%# Eval("Metadata.Class") %>' Width="150px" />
                    <asp:Label runat="server" Text='<%# Eval("Metadata.Quantity") %>' Width="250px" />
                    <asp:Label runat="server" Text='<%# ((Jhu.Graywulf.Schema.Unit)Eval("Metadata.Unit")).ToHtml() %>' Width="150px" />
                </div>
                <div class="gw-list-row">
                    <!--<span style="width: 32px"></span>-->
                    <span style="width: 180px"></span>
                    <asp:Label runat="server" Text='<%# Eval("Metadata.Summary") %>' CssClass="gw-list-multiline-span" />
                </div>
            </div>
        </ItemTemplate>
    </jgwc:MultiSelectListView>
</div>
