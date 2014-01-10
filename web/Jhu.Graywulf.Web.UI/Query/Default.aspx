<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Query.Default" CodeBehind="Default.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <link rel="stylesheet" href="../Scripts/CodeMirror/lib/codemirror.css">
    <script language="javascript">
        function refreshResults() {
            setTimeout(loadResults, 1000);
        }

        function loadResults() {
            $("#results").load("Results.aspx");
        }
    </script>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <div id="ToolbarDiv">
        <jgwc:Toolbar ID="Toolbar1" runat="server">
            <jgwc:ToolbarButton ID="Check" runat="server" Text="syntax check" OnClick="Check_Click" />
            <jgwc:ToolbarButton ID="ExecuteQuick" runat="server" OnClick="ExecuteQuick_Click"
                Text="quick execute" />
            <jgwc:ToolbarButton ID="ExecuteLong" runat="server" Text="execute" OnClick="ExecuteLong_Click" />
            <jgwc:ToolbarElement ID="ToolbarElement1" runat="server" Style="width: 120px">
                <asp:Label ID="OutputTableLabel" runat="server" Text="Output table:" /><br />
                <asp:TextBox ID="OutputTable" runat="server" CssClass="ToolbarControl" Width="120px"></asp:TextBox>
            </jgwc:ToolbarElement>
            <jgwc:ToolbarElement ID="ToolbarElement2" runat="server">
                <asp:Label ID="CommentsLabel" runat="server" Text="Comments:" /><br />
                <asp:TextBox ID="Comments" runat="server" CssClass="ToolbarControl" Style="width: 100%;
                    box-sizing: border-box;"></asp:TextBox>
            </jgwc:ToolbarElement>
        </jgwc:Toolbar>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <div runat="server" class="dock-bottom" visible="false" style="border: 1px solid #000000;"
                id="ResultsDiv">
                <div style="height: 150px; overflow: auto" id="results">
                </div>
                <script language="javascript">
                    loadResults();
                </script>
            </div>
            <div class="dock-bottom">
                <table class="Toolbar">
                    <tr class="Toolbar">
                        <td style="width: 40%">
                            Status:
                            <asp:Label ID="Message" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td style="width: 20%; text-align: center">
                            <asp:LinkButton ID="CloseResults" Visible="false" runat="server" OnClick="CloseResults_Click">close results pane</asp:LinkButton>&nbsp;&nbsp;&nbsp;
                        </td>
                        <td style="width: 80%; text-align: right; vertical-align: middle">
                            <asp:CheckBox runat="server" ID="SelectedOnly" Text="Execute selected only" />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="dock-fill" style="border: 1px solid #000000" id="EditorDiv">
                <jgwc:CodeMirror runat="server" ID="Query" Mode="text/x-sql" Theme="default" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
