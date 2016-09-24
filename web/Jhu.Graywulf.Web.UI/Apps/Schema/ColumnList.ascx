<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.ColumnList"
    CodeBehind="ColumnList.ascx.cs" %>
<jgwc:MultiSelectGridView runat="server" ID="listView" SelectionMode="Multiple" DataKeyNames="Name"
    AutoGenerateColumns="false">
    <Columns>
        <jgwc:BoundField DataField="Name" HeaderText="Name" ItemStyle-Width="180px"/>
        <jgwc:BoundField DataField="DataType.TypeNameWithLength" HeaderText="Type" ItemStyle-Width="100px" />
        <jgwc:BoundField DataField="DataType.ByteSize" HeaderText="Bytes" ItemStyle-Width="30px" ItemStyle-HorizontalAlign="Right" />
        <jgwc:BoundField DataField="Metadata.Class" HeaderText="Class" ItemStyle-Width="150px" />
        <jgwc:BoundField DataField="Metadata.Quantity" HeaderText="Quantity" ItemStyle-Width="150px" />
        <jgwc:BoundField DataField="Metadata.Unit" HeaderText="Unit" ItemStyle-Width="75px" />
        <jgwc:BoundField DataField="Metadata.Summary" HeaderText="Summary" ItemStyle-CssClass="GridViewSpan" />
    </Columns>
</jgwc:MultiSelectGridView>
