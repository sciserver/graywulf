<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Common.Feedback" CodeBehind="Feedback.aspx.cs" %>

<%@ Register TagPrefix="jgwc" Namespace="Jhu.Graywulf.Web.Controls" Assembly="Jhu.Graywulf.Web.Controls" %>
<asp:Content ContentPlaceHolderID="middle" runat="Server">
    <jgwc:Form ID="FeedbackForm" runat="server" Text="We appreciate your feedback!" SkinID="Feedback">
        <formtemplate>
            <p>
                Please use this form to send email to the site operators.</p>
            <table class="FormTable">
                <tr runat="server" id="NameRow">
                    <td class="FormLabel" style="width: 80px">
                        <asp:Label runat="server" ID="NameLabel">Your name:</asp:Label>
                    </td>
                    <td class="FormField" style="width: auto">
                        <asp:TextBox runat="server" ID="Name" CssClass="FormField" />
                        <asp:RequiredFieldValidator runat="server" ID="NameRequiredValidator" ControlToValidate="Name"
                            Display="Dynamic" ErrorMessage="<br /> Please fill in your name." />
                    </td>
                </tr>
                <tr runat="server" id="EmailRow">
                    <td class="FormLabel" style="width: 80px">
                        <asp:Label runat="server" ID="EmailLabel">Your e-mail address:</asp:Label>
                    </td>
                    <td class="FormField" style="width: auto">
                        <asp:TextBox runat="server" ID="Email" CssClass="FormField" />
                        <asp:RequiredFieldValidator runat="server" ID="EmailRequiredValidator" ControlToValidate="Email"
                            Display="Dynamic" ErrorMessage="<br /> Please fill in your e-mail address." />
                        <asp:RegularExpressionValidator runat="server" ID="EmailFormatValidator" ControlToValidate="Email"
                            Display="Dynamic" ErrorMessage="<br /> Invalid e-mail format" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel" style="width: 80px">
                        <asp:Label runat="server" ID="SubjectLabel">Subject:</asp:Label>
                    </td>
                    <td class="FormField" style="width: auto">
                        <asp:DropDownList runat="server" ID="Subject" CssClass="FormField">
                            <asp:ListItem Value="Question">Question about site</asp:ListItem>
                            <asp:ListItem Value="Error">Error report</asp:ListItem>
                            <asp:ListItem Value="Feature">Suggestion of new feature</asp:ListItem>
                            <asp:ListItem Value="Positive">Positive feedback</asp:ListItem>
                            <asp:ListItem Value="Negative">Negative feedback</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel" style="width: 80px">
                        <asp:Label runat="server" ID="TableNameLabel">Comments:</asp:Label>
                    </td>
                    <td class="FormField" style="width: auto">
                        <asp:TextBox ID="Comments" TextMode="MultiLine" Rows="15" runat="server" CssClass="FormField"
                            Width="100%"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="CommentsRequiredValidator" ControlToValidate="Comments"
                            Display="Dynamic" ErrorMessage="<br />Please write a message." />
                    </td>
                </tr>
            </table>
            <ul>
                <li runat="server" id="ErrorsIncluded" visible="false">The feedback message will include
                    detailed error information.</li>
                <li runat="server" id="SpecifySpace" visible="false">Please specify the approximate
                    amount of additional MyDB space you need. Because of the limited user data space
                    available, MyDB space requests are not guaranteed to be accepted.</li>
            </ul>
        </formtemplate>
        <buttonstemplate>
            <asp:Button ID="Ok" runat="server" Text="Ok" OnCommand="Ok_Click" CssClass="FormButton" />
            <asp:Button ID="Cancel" runat="server" OnClick="Cancel_Click" CausesValidation="False"
                Text="Cancel" CssClass="FormButton" />
        </buttonstemplate>
    </jgwc:Form>
    <jgwc:Form runat="server" ID="SuccessForm" Visible="false" SkinID="FeedbackSent">
        <formtemplate>
            <p>
                You feedback has been successfully sent.</p>
        </formtemplate>
        <buttonstemplate>
            <asp:Button ID="Back" runat="server" Text="Back" OnCommand="Back_Click" CssClass="FormButton"
                CausesValidation="false" />
        </buttonstemplate>
    </jgwc:Form>
</asp:Content>
