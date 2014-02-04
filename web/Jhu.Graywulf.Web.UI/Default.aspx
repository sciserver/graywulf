<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Jhu.Graywulf.Web.UI.Default" %>

<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="server">
    <jgwc:Form runat="server" ID="WelcomeForm" SkinID="Welcome">
        <FormTemplate>
            <p>
                You need to register and sign in before using this web site.
                You will be redirected to the sign in page by clicking any
                of the menu buttons above.
            </p>
        </FormTemplate>
        <ButtonsTemplate>
        </ButtonsTemplate>
    </jgwc:Form>
</asp:Content>
