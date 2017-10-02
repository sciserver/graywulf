<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Api.Default"
    MasterPageFile="~/App_Masters/Basic/UI.master" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <div class="toolbar">
        <a href="<%= ResolveUrl(GetUrl()) %>">api home</a>
        <div runat="server" id="serviceListDiv" style="min-width: 140px">
            <asp:Label ID="serviceListLabel" runat="server" Text="Service:" /><br />
            <asp:DropDownList ID="serviceList1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ServiceList_SelectedIndexChanged" />
        </div>
        <div runat="server" id="toolbarSpan" class="span"></div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="middle">

    <asp:Panel runat="server" ID="iframePanel" Visible="false" CssClass="dock-container dock-fill">
        <iframe runat="server" id="iframe" class="dock-fill" style="border: 0;"></iframe>
    </asp:Panel>
    <jgwuc:Form runat="server" ID="servicesForm" Text="Application Programming Interface"
        SkinID="Api">
        <FormTemplate>
            <p>
                You can use the following REST services to access functionality from your scripts
                and programs.
            </p>
            <ul runat="server" id="serviceList2">
            </ul>
            <p>
                Please note, that you need to be registered and signed in in order to access the
                service interfaces.
            </p>
        </FormTemplate>
        <ButtonsTemplate>
        </ButtonsTemplate>
    </jgwuc:Form>
</asp:Content>
