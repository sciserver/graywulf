<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.IndexList" Codebehind="IndexList.ascx.cs" %>
<jgwc:MultiSelectGridView runat="server" ID="listView" SelectionMode="Multiple" DataKeyNames="IndexName"
    AutoGenerateColumns="false">
    <Columns>
        <jgwc:BoundField DataField="IndexName" HeaderText="Name" ItemStyle-Width="240px" />
        <jgwc:BoundField DataField="ColumnListDisplayString" HeaderText="Columns" ItemStyle-CssClass="GridViewSpan" />
        <jgwc:BoundField DataField="IncludedColumnListDisplayString" HeaderText="Included Columns" ItemStyle-CssClass="GridViewSpan" />
    </Columns>
</jgwc:MultiSelectGridView>
