<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.ParameterList"
    CodeBehind="ParameterList.ascx.cs" %>
<jgwc:MultiSelectGridView runat="server" ID="listView" SelectionMode="Multiple" DataKeyNames="Name"
    AutoGenerateColumns="false">
    <Columns>
        <jgwc:BoundField DataField="Name" HeaderText="Name" ItemStyle-Width="180px" />
        <jgwc:BoundField DataField="DataType.TypeNameWithLength" HeaderText="Type" ItemStyle-Width="100px" />
        <jgwc:BoundField DataField="DataType.ByteSize" HeaderText="Size" ItemStyle-Width="30px"
            ItemStyle-HorizontalAlign="Right" />
        <jgwc:BoundField DataField="Direction" HeaderText="Direction" ItemStyle-Width="50px" />
        <jgwc:BoundField DataField="Metadata.Class" HeaderText="Content" ItemStyle-Width="100px" />
        <jgwc:BoundField DataField="Metadata.Quantity" HeaderText="Content" ItemStyle-Width="100px" />
        <jgwc:BoundField DataField="Metadata.Unit" HeaderText="Unit" ItemStyle-Width="30px" />
        <jgwc:BoundField DataField="Metadata.Summary" HeaderText="Summary" ItemStyle-CssClass="GridViewSpan" />
    </Columns>
</jgwc:MultiSelectGridView>
