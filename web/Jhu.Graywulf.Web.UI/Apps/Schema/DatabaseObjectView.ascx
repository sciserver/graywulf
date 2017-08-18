<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatabaseObjectView.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.DatabaseObjectView" %>
<div class="dock-fill dock-scroll">
    <h3>
        <asp:Label ID="objectTypeLabel" runat="server" Text="Table:" />:&nbsp;
    <asp:Label ID="datasetNameLabel" runat="server" />:<!--
 --><asp:Label ID="schemaNameLabel" runat="server" />.<!--
 --><asp:Label ID="objectNameLabel" runat="server" />
    </h3>
    <asp:Panel runat="server" ID="summaryPanel">
        <h3>Summary</h3>
        <p>
            <asp:Literal runat="server" ID="summaryLabel" Mode="PassThrough" />
        </p>
    </asp:Panel>
    <asp:Panel runat="server" ID="remarksPanel">
        <h3>Remarks</h3>
        <p>
            <asp:Literal runat="server" ID="remarksLabel" Mode="PassThrough" />
        </p>
    </asp:Panel>
    <asp:Panel runat="server" ID="examplePanel">
        <h3>Example</h3>
        <p>
            <asp:Literal runat="server" ID="exampleLabel" Mode="PassThrough" />
        </p>
    </asp:Panel>
    <jgwc:HtmlIncluder runat="server" ID="docPage" />
</div>
