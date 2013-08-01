<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="Jhu.Graywulf.Web.Auth.SignIn"
    MasterPageFile="~/Auth.master" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <jgwc:Form runat="server" ID="SignInForm" SkinID="SignIn" Text="Welcome">
        <FormTemplate>
            <p>
                To start using the services, please sign in with your existing credentials.
            </p>
            <table class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label runat="server" ID="UsernameLabel">User name:</asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="Username" runat="server" CssClass="FormField"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UsernameRequiredValidator" runat="server" Display="Dynamic"
                            ErrorMessage="<br />Username is required" ControlToValidate="Username" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label runat="server" ID="PasswordLabel">Password:</asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="FormField"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequiredValidator" runat="server" Display="Dynamic"
                            ErrorMessage="<br />Password is required" ControlToValidate="Password" />
                        <asp:CustomValidator ID="PasswordValidator" runat="server" Display="Dynamic" ErrorMessage="<br />Invalid User name or Password"
                            OnServerValidate="PasswordValidator_ServerValidate" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        &nbsp;
                    </td>
                    <td class="FormField">
                        <asp:CheckBox ID="Remember" runat="server" Text="Remember me on this computer" />
                    </td>
                </tr>
            </table>
            <ul>
                <li>If not registered yet,
                    <asp:HyperLink runat="server" ID="RegisterLink">create a new
                    account</asp:HyperLink>.</li>
                <li>If you have already registered, proceed to
                    <asp:HyperLink runat="server" ID="ActivateLink">
                    account activation</asp:HyperLink>.</li>
                <li>If you have forgotten you password,
                    <asp:HyperLink runat="server" ID="ResetLink">
                    request a password reset</asp:HyperLink>.</li>
            </ul>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button runat="Server" ID="Ok" Text="Sign in" CssClass="FormButton" OnClick="Ok_Click" />
            <asp:Button runat="Server" ID="Register" Text="Register" CssClass="FormButton" OnClick="Register_Click" />
        </ButtonsTemplate>
    </jgwc:Form>
</asp:Content>
