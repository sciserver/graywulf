<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageError.aspx.cs" Inherits="Jhu.Graywulf.Web.UI.Error"
    MasterPageFile="~/App_Masters/Basic.master" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <jgwc:Form ID="ErrorForm" runat="server" Text="Oooooops!" SkinID="Error">
        <FormTemplate>
            <p>
                Unexpected error happened:</p>
            <p>
                <asp:Label runat="server" ID="Message" /></p>
            <ul>
                <li>The error has been recorded in the log.</li>
                <li>If the error seems permanent, you can
                    <asp:HyperLink runat="Server" ID="FeedbackLink">send an inquiry</asp:HyperLink>
                    to the site administrators.</li>
            </ul>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button runat="server" ID="Ok" Text="Ok" CssClass="FormButton" OnClick="Ok_Click" />
        </ButtonsTemplate>
    </jgwc:Form>
</asp:Content>
