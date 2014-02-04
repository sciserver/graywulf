<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Jhu.Graywulf.Web.UI.Api.Default"
    MasterPageFile="~/UI.master" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <jgwc:Form runat="server" Text="Application Programming Interface" SkinID="Api">
                <FormTemplate>
                    <p>
                        You can use the following REST services to access functionality from your scripts
                        and programs.</p>
                    <ul>
                        <li><a href="Jobs/help">Jobs</a>: schedule queries, table import and export jobs.</li>
                        <li><a href="Tables/help">Table</a>: Upload and download tables directly.</li>
                        <li><a href="Schema/help">Schema</a>: Browse the schema.</li>
                    </ul>
                    <p>
                        Please note, that you need to be registered and signed in in order to access the
                        service interfaces.
                    </p>
                </FormTemplate>
                <ButtonsTemplate>
                </ButtonsTemplate>
            </jgwc:Form>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
