<%@ Control Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.DbObjectDetails"
    CodeBehind="DbObjectDetails.ascx.cs" %>
<%@ Register Src="ColumnList.ascx" TagName="ColumnList" TagPrefix="list" %>
<%@ Register Src="ParameterList.ascx" TagName="ParameterList" TagPrefix="list" %>
<%@ Register Src="IndexList.ascx" TagName="IndexList" TagPrefix="list" %>
<div class="dock-top">
    <jgwc:MultiViewTabHeader runat="server" ID="tabs" MultiViewID="multiView" />
</div>
<div class="dock-fill dock-container TabFrame">
    <div class="dock-bottom">
        <p class="FormMessage">
        </p>
        <p class="FormButtons">
            <asp:Button ID="peek" runat="server" Text="Peek" Visible="false" CssClass="FormButton" />
            <asp:Button ID="export" runat="server" Text="Export" Visible="false" OnClick="Export_Click"
                CssClass="FormButton" />
            <asp:Button ID="rename" runat="server" Text="Rename" Visible="false" OnClick="Rename_Click"
                CssClass="FormButton" />
            <asp:Button ID="primaryKey" runat="server" Text="Primary Key" OnClick="PrimaryKey_Click"
                CssClass="FormButton" />
            <asp:Button ID="drop" runat="server" Text="Drop" Visible="false" OnClick="Drop_Click"
                CssClass="FormButton" />
        </p>
    </div>
    <div class="dock-fill dock-scroll">
        <h3>
            <asp:Label ID="FullyQualifiedNameLabel" runat="server" Text="Table:"></asp:Label>:&nbsp;<asp:Label
                ID="DatasetNameLabel" runat="server" />:<asp:Label ID="SchemaNameLabel" runat="server" />.<asp:Label
                    ID="DbObjectNameLabel" runat="server" /></h3>
        <asp:MultiView runat="server" ID="multiView" ActiveViewIndex="0">
            <jgwc:TabView runat="server" ID="detailsTab" Text="Details">
                <p>
                    <asp:Label runat="server" ID="SummaryLabel"></asp:Label>
                </p>
                <asp:Panel runat="server" ID="RemarksPanel">
                    <h3>
                        Remarks</h3>
                    <p>
                        <asp:Label runat="server" ID="RemarksLabel"></asp:Label>
                    </p>
                </asp:Panel>
                <asp:Panel runat="server" ID="ExamplePanel">
                    <h3>
                        Example</h3>
                    <p>
                        <asp:Label runat="server" ID="ExampleLabel"></asp:Label>
                    </p>
                </asp:Panel>
            </jgwc:TabView>
            <jgwc:TabView runat="server" ID="columnsTab" Text="Columns">
                <list:ColumnList ID="columnsList" runat="server" />
            </jgwc:TabView>
            <jgwc:TabView runat="server" ID="parametersTab" Text="Parameters">
                <list:ParameterList ID="parametersList" runat="server" />
            </jgwc:TabView>
            <jgwc:TabView runat="server" ID="indexesTab" Text="Indexes">
                <list:IndexList ID="indexesList" runat="server" />
            </jgwc:TabView>
        </asp:MultiView>
    </div>
</div>
