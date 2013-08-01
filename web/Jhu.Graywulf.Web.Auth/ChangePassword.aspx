<%@ Page Title="" Language="C#" MasterPageFile="~/Auth.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.Auth.ChangePassword" CodeBehind="ChangePassword.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <jgwc:Form runat="server" ID="ChangePasswordForm" SkinID="ChangePassword" Text="Change password">
        <FormTemplate>
            <p>
                You have requested password reset. Please enter a new password below.</p>
            <table class="FormTable">
                <tr runat="server" id="OldPasswordRow">
                    <td class="FormLabel">
                        <asp:Label ID="OldPasswordLabel" runat="server" Text="Old password:" />
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="OldPassword" runat="server" TextMode="Password" CssClass="FormField" />
                        <asp:RequiredFieldValidator ID="OldPasswordRequiredValidator" runat="server" ControlToValidate="OldPassword"
                            Display="Dynamic" ErrorMessage="<br />Field is required" CssClass="FormValidator" />
                        <asp:CustomValidator ID="OldPasswordValidator" runat="server" ControlToValidate="OldPassword"
                            Display="Dynamic" ErrorMessage="<br />Old password is invalid" CssClass="FormValidator" OnServerValidate="OldPasswordValidator_ServerValidate" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="PasswordLabel" runat="server" Text="Password:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="FormField"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequiredValidator" runat="server" ControlToValidate="Password"
                            Display="Dynamic" ErrorMessage="<br />Field is required" CssClass="FormValidator"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="ConfirmPasswordLabel" runat="server" Text="Confirm password:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" CssClass="FormField"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="ConfirmPasswordRequiredValidator" runat="server"
                            ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="<br />Field is required"
                            CssClass="FormValidator"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="ConfirmPasswordValidator" runat="server" ControlToValidate="ConfirmPassword"
                            Display="Dynamic" ErrorMessage="<br />Password and confirmation must match" OnServerValidate="ConfirmPasswordValidator_ServerValidate"
                            CssClass="FormValidator"></asp:CustomValidator>
                    </td>
                </tr>
            </table>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Ok" runat="server" OnClick="Ok_Click" Text="OK" CssClass="FormButton" />
        </ButtonsTemplate>
    </jgwc:Form>
    <jgwc:Form runat="server" ID="SuccessForm" SkinID="ChangePassword" Text="Password changed" Visible="false">
        <FormTemplate>
            <p>
                Your password has been reset successfuly.</p>
            <ul>
                <li>Please, <a href="SignIn.aspx?ReturnUrl=<%= ReturnUrl %>">proceed to sign in</a>.</li></ul>
        </FormTemplate>
        <ButtonsTemplate>
        </ButtonsTemplate>
    </jgwc:Form>
</asp:Content>
