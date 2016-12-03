<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Query.Default" CodeBehind="Default.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <asp:UpdatePanel ID="toolbarPanel" runat="server">
        <ContentTemplate>
            <div class="toolbar">
                <div class="span">
                    <asp:Label ID="commentsLabel" runat="server" Text="Query job comments:" /><br />
                    <asp:TextBox ID="comments" runat="server"></asp:TextBox>
                </div>
                <div style="min-width: 72px; text-align: center">
                    <asp:Label runat="server" ID="selectedOnlyLabel" Text="Selection&lt;br /&gt; only" />&nbsp;
                    <asp:CheckBox runat="server" ID="selectedOnly" />
                </div>
                <asp:LinkButton ID="check" runat="server" Text="syntax check" OnClick="Check_Click" />
                <asp:LinkButton ID="executeQuick" runat="server" OnClick="ExecuteQuick_Click"
                    Text="quick execute" />
                <asp:LinkButton ID="executeLong" runat="server" Text="execute" OnClick="ExecuteLong_Click" />
            </div>
            <jgwc:MessageBar runat="server" ID="messageBar" Visible="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <div class="dock-fill dock-container" style="border: 1px solid #000000" id="EditorDiv">
        <jgwc:CodeMirror runat="server" ID="Query" Mode="text/x-sql" Theme="default" CssClass="dock-fill" Width="100%" Height="100%" />
    </div>
</asp:Content>
