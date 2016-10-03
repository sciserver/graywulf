<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Query.Default" CodeBehind="Default.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <script lang="javascript">
        function refreshResults() {
            setTimeout(loadResults, 1000);
        }

        function loadResults() {
            $("#results").load("Results.aspx");
        }
    </script>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="toolbarPanel" class="ToolbarFrame">
                <div class="Toolbar">
                    <div class="ToolbarElement" style="width: auto">
                        <asp:Label ID="commentsLabel" runat="server" Text="Query job comments:" /><br />
                        <asp:TextBox ID="comments" runat="server" CssClass="ToolbarControl" Style="width: 100%;"></asp:TextBox>
                    </div>
                    <div class="ToolbarElement" style="width: 72px; text-align: center">
                        <asp:Label runat="server" ID="selectedOnlyLabel" Text="Selection&lt;br /&gt; only" />&nbsp;
                <asp:CheckBox runat="server" ID="selectedOnly" />
                    </div>
                    <asp:LinkButton ID="check" runat="server" Text="syntax check" CssClass="ToolbarButton" OnClick="Check_Click" />
                    <asp:LinkButton ID="executeQuick" runat="server" CssClass="ToolbarButton" OnClick="ExecuteQuick_Click"
                        Text="quick execute" />
                    <asp:LinkButton ID="executeLong" runat="server" Text="execute" CssClass="ToolbarButton" OnClick="ExecuteLong_Click" />
                </div>
            </asp:Panel>
            <asp:Panel runat="server" ID="messagePanel" CssClass="ToolbarMessage" Visible="false">
                <asp:Label runat="server" ID="message" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <div runat="server" class="dock-bottom" visible="false" style="border: 1px solid #000000;"
                id="ResultsDiv">
                <div style="height: 150px; overflow: auto" id="results">
                </div>
                <script lang="javascript">
                    loadResults();
                </script>
            </div>
            <div class="dock-bottom">
                <table class="Toolbar">
                    <tr class="Toolbar">
                        <td style="width: 40%">Status:
                        </td>
                        <td style="width: 20%; text-align: center">
                            <asp:LinkButton ID="CloseResults" Visible="false" runat="server" OnClick="CloseResults_Click">close results pane</asp:LinkButton>&nbsp;&nbsp;&nbsp;
                        </td>
                        <td style="width: 80%; text-align: right; vertical-align: middle"></td>
                    </tr>
                </table>
            </div>
            <div class="dock-fill dock-container" style="border: 1px solid #000000" id="EditorDiv">
                <jgwc:CodeMirror runat="server" ID="Query" Mode="text/x-sql" Theme="default" CssClass="dock-fill" Width="100%" Height="100%" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
