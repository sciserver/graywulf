<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.QueueDefinitionDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="QueueDefinitionDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="MaxOutstandingJobsLabel" runat="server" Text="Max Outstanding Jobs:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="MaxOutstandingJobs" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="TimeoutLabel" runat="server" Text="Timeout:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Timeout" runat="server"></asp:Label>
                sec
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
