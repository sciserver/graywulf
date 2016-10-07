<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignOut.aspx.cs" Inherits="Jhu.Graywulf.Web.Admin.User.SignOut"
    MasterPageFile="~/Admin.master" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <jgwuc:Form runat="server" Text="Sign out" SkinID="SignOut">
        <FormTemplate>
            <p>You have successfully signed out from <asp:Label runat="server" ID="ShortTitle" />.</p>
        </FormTemplate>
        <ButtonsTemplate>
            <input type="button" runat="server" value="Ok" ID="Ok" class="FormButton" />
        </ButtonsTemplate>
    </jgwuc:Form>
</asp:Content>