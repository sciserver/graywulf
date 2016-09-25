<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Auth.User"
    MasterPageFile="~/App_Masters/Basic/Basic.master" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <jgwuc:Form runat="server" ID="UserForm" SkinID="User">
        <FormTemplate>
            <table runat="server" id="UsernameTable" class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="UsernameLabel" runat="server" Text="User name:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="Username" runat="server" CssClass="FormField"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="UsernameFormatValidator" runat="server" ControlToValidate="Username"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;Username is invalid" ValidationExpression="^[a-zA-Z0-9@\._-]+" />
                        <asp:CustomValidator ID="DuplicateUsernameValidator" runat="server" ControlToValidate="Username"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;Username is already in use" OnServerValidate="DuplicateUsernameValidator_ServerValidate" />
                        <asp:RequiredFieldValidator ID="PasswordRequiredValidator0" runat="server" ControlToValidate="Username"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;User name is required" />
                    </td>
                </tr>
            </table>
            <table runat="server" id="DetailsTable" class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="FirstName" runat="server" CssClass="FormField"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequiredValidator1" runat="server" ControlToValidate="FirstName"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;This field is required" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="MiddleNameLabel" runat="server" Text="Middle Name:"></asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="MiddleName" runat="server" CssClass="FormField"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="LastName" runat="server" CssClass="FormField"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequiredValidator2" runat="server" ControlToValidate="LastName"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;This field is required" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="EmailLabel" runat="server" Text="E-mail Address:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="Email" runat="server" CssClass="FormField" Width="100%"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="EmailFormatValidator" runat="server" Display="Dynamic"
                            ErrorMessage="&lt;br /&gt;Invalid format" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                            ControlToValidate="Email"></asp:RegularExpressionValidator>
                        <asp:RequiredFieldValidator ID="EmailRequiredValidator" runat="server" ControlToValidate="Email"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;This field is required"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="EmailDuplicateValidator" runat="server" ControlToValidate="Email"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;E-mail address is already in use." OnServerValidate="DuplicateEmailValidator_ServerValidate" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="CompanyLabel" runat="server" Text="Institution:"></asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="Company" runat="server" CssClass="FormField" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="AddressLabel" runat="server" Text="Address:"></asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="Address" runat="server" CssClass="FormField" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="WorkPhoneLabel" runat="server" Text="Phone:"></asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="WorkPhone" runat="server" CssClass="FormField"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <table runat="server" id="PasswordTable" class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="PasswordLabel" runat="server" Text="Password:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="Password" runat="server" CssClass="FormField" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequiredValidator" runat="server" ControlToValidate="Password"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;Password is required" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="ConfirmPasswordLabel" runat="server" Text="Confirm password:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="FormField" TextMode="Password"></asp:TextBox>
                        <asp:CustomValidator ID="ConfirmPasswordValidator" runat="server" ControlToValidate="ConfirmPassword"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;Password and confirmation do not match"
                            OnServerValidate="ConfirmPasswordValidator_ServerValidate" />
                    </td>
                </tr>
            </table>
            <table runat="server" id="CaptchaTable" class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="CaptchaLabel" runat="server" Text="Captcha:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <jgwuc:CaptchaImage runat="server" Height="64px" Width="200px" CssClass="Captcha"
                            Digits="8" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        &nbsp;
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="Captcha" runat="server" CssClass="FormField"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="CaptchaRequiredValidator" runat="server" ControlToValidate="Captcha"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;Enter captcha" />
                        <jgwuc:CaptchaValidator ID="CaptchaValidator" runat="server" ControlToValidate="Captcha"
                            Display="Dynamic" ErrorMessage="&lt;br /&gt;Reenter captcha" />
                    </td>
                </tr>
            </table>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="OK" runat="server" Text="OK" OnClick="OK_Click" CssClass="FormButton" />
            &nbsp;<asp:Button ID="Cancel" runat="server" CausesValidation="False" Text="Cancel"
                OnClick="Cancel_Click" CssClass="FormButton" />
            <span runat="server" id="ChangePasswordPanel">|
                <asp:Button ID="ChangePassword" runat="server" CausesValidation="false" Text="Change password"
                    OnClick="ChangePassword_Click" CssClass="FormButton" /></span>
        </ButtonsTemplate>
    </jgwuc:Form>
    <jgwuc:Form runat="server" ID="SuccessForm" SkinID="UserForm" Text="User data changed"
        Visible="false">
        <FormTemplate>
            <p>
                Your user information has been updated successfuly.</p>
        </FormTemplate>
        <ButtonsTemplate>
            <input type="button" runat="server" id="SuccessOK" value="OK" class="FormButton" />
        </ButtonsTemplate>
    </jgwuc:Form>
</asp:Content>
