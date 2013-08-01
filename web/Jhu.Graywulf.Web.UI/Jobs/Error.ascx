<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Error.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Jobs.Error" %>
<jgwc:Form ID="Form1" runat="server" Text="Error details" SkinID="ErrorJob">
    <FormTemplate>
        <p>
            The following message might contain useful information on why the job failed.
        </p>
        <table class="FormTable">
            <tr>
                <td class="FormLabel">
                    <asp:Label runat="server" Text="Error message:"></asp:Label>
                </td>
                <td>
                    <asp:Label runat="server" ID="ExceptionMessage"></asp:Label>
                </td>
            </tr>
        </table>
        <ul>
            <li>If you think this error is due to a bug in the system, please
                <asp:HyperLink runat="server" ID="SendInquiry">send an inquiry</asp:HyperLink> to the system administrator.</li>
        </ul>
    </FormTemplate>
    <ButtonsTemplate>
        <asp:Button runat="Server" ID="Inquiry" Text="Send inquiry" CssClass="FormButton"
            OnClick="Inquiry_Click" />
    </ButtonsTemplate>
</jgwc:Form>
