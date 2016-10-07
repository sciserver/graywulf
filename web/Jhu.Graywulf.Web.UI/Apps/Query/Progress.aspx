<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true" CodeBehind="Progress.aspx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Query.Progress" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <asp:UpdatePanel runat="server" class="dock-fill">
        <ContentTemplate>
            <asp:Panel runat="server" ID="statusPanel" CssClass="dock-fill">
                <jgwuc:Form runat="Server" Text="Executing query" SkinID="JobDetails">
                    <FormTemplate>
                        <p><asp:Label runat="server" ID="statusLabel" Text="Query status:" /> <jgwuc:JobStatus runat="server" ID="status" /></p>
                        <p><asp:Label ID="exceptionLabel" runat="server" Text="Error:" Visible="false"></asp:Label></p>
                        <p><asp:Label ID="exception" runat="server" Text="Label" Visible="false"></asp:Label></p>
                    </FormTemplate>
                    <ButtonsTemplate>
                    </ButtonsTemplate>
                </jgwuc:Form>
                <asp:Timer runat="server" ID="statusTimer" Interval="2000" OnTick="StatusTimer_Tick" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
