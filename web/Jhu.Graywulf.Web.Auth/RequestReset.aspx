<%@ Page Title="" Language="C#" MasterPageFile="~/Auth.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.Auth.RequestReset" CodeBehind="RequestReset.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <jgwc:Form runat="server" ID="RequestResetForm" SkinID="RequestReset" Text="Password reset">
        <FormTemplate>
            <p>
                To request a password reset, please enter your e-mail address.</p>
            <table class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="EmailLabel" runat="server" Text="Email:"></asp:Label>
                        *
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="Email" runat="server" CssClass="FormField"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="EmailRequiredValidator" runat="server" ControlToValidate="Email"
                            Display="Dynamic" ErrorMessage="<br />Field is required" CssClass="FormValidator"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="EmailFormatValidator" runat="server" ControlToValidate="Email"
                            Display="Dynamic" ErrorMessage="<br />Invalid format" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                            CssClass="FormValidator"></asp:RegularExpressionValidator>
                        <asp:CustomValidator ID="EmailValidator" runat="server" ControlToValidate="Email"
                            ErrorMessage="<br />Email address not found" OnServerValidate="EmailValidator_ServerValidate"
                            CssClass="FormValidator"></asp:CustomValidator>
                    </td>
                </tr>
            </table>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Ok" runat="server" OnClick="Ok_Click" Text="OK" CssClass="FormButton" />
            <asp:Button ID="Cancel" runat="server" CausesValidation="False" OnClick="Cancel_Click"
                Text="Cancel" CssClass="FormButton" />
        </ButtonsTemplate>
    </jgwc:Form>
    <jgwc:Form runat="server" ID="SuccessForm" SkinID="RequestReset" Text="Password reset" Visible="false">
        <FormTemplate>
            <p>
                An e-mail to the address has been sent. Please follow the instructions
                in the e-mail.</p>
            <ul>
                <li><asp:HyperLink runat="server" ID="SingInLink">Proceed to sign in</asp:HyperLink>.</li></ul>
        </FormTemplate>
        <ButtonsTemplate>
        </ButtonsTemplate>
    </jgwc:Form>
</asp:Content>
