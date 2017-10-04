<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageError.aspx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Common.Error"
    MasterPageFile="~/App_Masters/Basic/Basic.master" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <jgwuc:Form ID="ErrorForm" runat="server" Text="Oooooops!" SkinID="Error">
        <FormTemplate>
            <p>
                Unexpected error happened:</p>
            <p>
                <asp:Label runat="server" ID="Message" /></p>
            <ul>
                <li>The error has been recorded in the log.</li>
                <li>If the error seems permanent, you can
                    <asp:LinkButton runat="Server" ID="inquiryLink" OnClick="Inquiry_Click">send an inquiry</asp:LinkButton>
                    to the site administrators.</li>
            </ul>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button runat="server" ID="ok" Text="Ok" CssClass="FormButton" OnClick="Ok_Click" />
            <asp:Button runat="server" ID="inquiry" Text="Send inquiry" CssClass="FormButton" OnClick="Inquiry_Click" />
        </ButtonsTemplate>
    </jgwuc:Form>
</asp:Content>
