<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Schema.ColumnList"
    CodeBehind="ColumnList.ascx.cs" %>
<jgwc:MultiSelectGridView runat="server" ID="listView" SelectionMode="Multiple" DataKeyNames="Name"
    AutoGenerateColumns="false">
    <Columns>
        <jgwc:BoundField DataField="Name" HeaderText="Name" ItemStyle-Width="180px"/>
        <jgwc:BoundField DataField="DataType.Name" HeaderText="Type" ItemStyle-Width="100px" />
        <jgwc:BoundField DataField="DataType.Size" HeaderText="Size" ItemStyle-Width="30px" ItemStyle-HorizontalAlign="Right" />
        <jgwc:BoundField DataField="Metadata.Content" HeaderText="Content" ItemStyle-Width="100px" />
        <jgwc:BoundField DataField="Metadata.Unit" HeaderText="Unit" ItemStyle-Width="30px" />
        <jgwc:BoundField DataField="Metadata.Summary" HeaderText="Summary" ItemStyle-CssClass="GridViewSpan" />
    </Columns>
</jgwc:MultiSelectGridView>
