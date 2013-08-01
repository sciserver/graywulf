<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.QueueInstanceForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="QueueInstanceForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="Form">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="QueueDefinitionLabel" runat="server" Text="Queue Definition:"></asp:Label>
                &nbsp;
            </td>
            <td class="FormField">
                <asp:DropDownList ID="QueueDefinition" runat="server" CssClass="FormField">
                </asp:DropDownList>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="MaxOutstandingJobsLabel" runat="server" Text="Max Outstanding Jobs:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="MaxOutstandingJobs" runat="server" CssClass="FormField"></asp:TextBox>
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