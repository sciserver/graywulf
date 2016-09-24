<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Api.Default"
    MasterPageFile="~/App_Masters/Basic/UI.master" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <jgwuc:Form runat="server" Text="Application Programming Interface" SkinID="Api">
                <FormTemplate>
                    <p>
                        You can use the following REST services to access functionality from your scripts
                        and programs.</p>
                    <ul runat="server" id="serviceList">
                    </ul>
                    <p>
                        Please note, that you need to be registered and signed in in order to access the
                        service interfaces.
                    </p>
                </FormTemplate>
                <ButtonsTemplate>
                </ButtonsTemplate>
            </jgwuc:Form>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
