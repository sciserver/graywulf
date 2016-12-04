<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.ParameterList"
    CodeBehind="ParameterList.ascx.cs" %>

<div class="dock-top gw-list-frame-top">
    <div class="gw-list-header">
        <div class="gw-list-row">
            <!--<span style="width: 32px"></span>-->
            <span style="width: 180px">parameter name</span>
            <span style="width: 150px">data type</span>
            <span style="width: 64px">byte size</span>
            <span style="width: 100px">direction</span>
            <span style="width: 150px">class</span>
            <span style="width: 150px">quantity</span>
            <span style="width: 150px">unit</span>
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
                    <asp:Label runat="server" Text='<%# Eval("Direction") %>' Width="100px" />
                    <asp:Label runat="server" Text='<%# Eval("Metadata.Class") %>' Width="150px" />
                    <asp:Label runat="server" Text='<%# Eval("Metadata.Quantity") %>' Width="150px" />
                    <asp:Label runat="server" Text='<%# Eval("Metadata.Unit") %>' Width="150px" />
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
