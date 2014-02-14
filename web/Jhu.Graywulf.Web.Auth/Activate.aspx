<%@ Page Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.Auth.Activate"
    MasterPageFile="~/Auth.master" CodeBehind="Activate.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <jgwc:Form runat="server" ID="ActivateUserForm" Text="Activate user account" SkinID="Activate">
        <FormTemplate>
            <p>
                When you registered to
                <%= Application[Jhu.Graywulf.Web.Constants.ApplicationShortTitle] %>
                an e-mail with an activation code was sent to your e-mail adderess. Please enter
                the activation below to begin using the service.</p>
            <ul>
                <li>If you have not registered yet,
                    <asp:HyperLink runat="server" ID="UserLink">go
                        to registration</asp:HyperLink>.</li>
                <li>If you have already activated your account,
                    <asp:HyperLink runat="server" ID="SignInLink">
                        proceed to sign in</asp:HyperLink>.</li>
            </ul>
            <table class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="ActivationCodeLabel" runat="server" Text="Activation code:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="ActivationCode" runat="server" CssClass="FormField"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="ActivationCodeRequiredValidator" runat="server" ControlToValidate="ActivationCode"
                            Display="Dynamic" ErrorMessage="<br />Field is required" />
                        <asp:CustomValidator ID="ActivationCodeValidator" runat="server" ControlToValidate="ActivationCode"
                            Display="Dynamic" ErrorMessage="<br />Invalid activation code" OnServerValidate="ActivationCodeValidator_ServerValidate" />
                    </td>
                </tr>
            </table>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="ActivateButton" runat="server" Text="Activate" OnClick="Activate_Click"
                CssClass="FormButton" />
        </ButtonsTemplate>
    </jgwc:Form>
    <jgwc:Form runat="server" ID="SuccessForm" Text="Activate user account" SkinID="ActivateSuccess"
        Visible="false">
        <FormTemplate>
            <p>
                Activation successful. Please, proceed to
                <asp:HyperLink runat="server" ID="SignInLink2">sign in</asp:HyperLink>.</p>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button runat="server" CssClass="FormButton" Text="Sign in" OnClick="SignIn_Click" />
        </ButtonsTemplate>
    </jgwc:Form>
</asp:Content>
