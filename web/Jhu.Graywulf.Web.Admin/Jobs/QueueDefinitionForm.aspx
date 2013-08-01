<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.QueueDefinitionForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="QueueDefinitionForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="Form">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="MaxOutstandingJobsLabel" runat="server" Text="Max Outstanding Jobs:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="MaxOutstandingJobs" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TimeoutLabel" runat="server" Text="Timeout:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="Timeout" runat="server" CssClass="FormFieldNarror"></asp:TextBox>
                sec
            </td>
        </tr>
    </table>
</asp:Content>
