<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.QueueInstanceDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="QueueInstanceDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="QueueDefinitionLabel" runat="server" Text="QueueDefinition:"></asp:Label>
            </td>
            <td class="FormField">
                &nbsp;<jgwac:EntityLink ID="QueueDefinition" Expression="[$Parent.Name]\[$Name]"
                    runat="server" />
            </td>
        </tr>
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
<asp:Content ID="Content5" runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="JobInstanceList" ChildrenType="JobInstance"
            EntityGroup="Jobs">
            <columns>
                <jgwc:BoundField DataField="JobDefinition.Name" HeaderText="Job Definition" />
                <asp:BoundField DataField="JobExecutionStatus" HeaderText="Execution Status" />
                <asp:BoundField DataField="ScheduleTime" HeaderText="Scheduled" />
                <asp:BoundField DataField="DateStarted" HeaderText="Started" />
                <asp:BoundField DataField="DateFinished" HeaderText="Finished" />
                <asp:BoundField DataField="DateCreated" HeaderText="Created" />
                <asp:BoundField DataField="DateModified" HeaderText="Modified" />
            </columns>
        </jgwac:EntityList>
    </jgwac:EntityChildren>
</asp:Content>

