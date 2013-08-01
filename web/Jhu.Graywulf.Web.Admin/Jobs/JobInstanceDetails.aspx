<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.JobInstanceDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="JobInstanceDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="JobDefinitionLabel" runat="server" Text="Job Definition:"></asp:Label>
            </td>
            <td class="FormField">
                &nbsp;<jgwac:EntityLink ID="JobDefinition" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="WorkflowTypeLabel" runat="server" Text="Workflow Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="WorkflowType" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DateStartedLabel" runat="server" Text="Started:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="DateStarted" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="DateFinishedLabel" runat="server" Text="Finished:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="DateFinished" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ExecutionStatusLabel" runat="server" Text="Execution Status:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ExecutionStatus" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ScheduleTypeLabel" runat="server" Text="Scheduling Method:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ScheduleType" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ScheduleTimeLabel" runat="server" Text="Scheduled at:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ScheduleTime" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="RecurringPeriodLabel" runat="server" Text="Recurring Period:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="RecurringPeriod" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="RecurringIntervalLabel" runat="server" Text="Recurring Interval:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="RecurringInterval" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="RecurringMaskLabel" runat="server" Text="Recurring Mask:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="RecurringMask" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ExceptionMessageLabel" runat="server" Text="Exception message:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="ExceptionMessage" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
    <asp:Panel runat="server" ID="AdminRequest" Visible="false">
        <h3>
            Admin Request</h3>
        <p>
            The workflow is suspended and there's an adminitrator request waiting.</p>
        <table cellpadding="0" cellspacing="0" class="Form">
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="AdminRequestTitleLabel" runat="server" Text="Title:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label ID="AdminRequestTitle" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="AdminRequestMessageLabel" runat="server" Text="Label"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label ID="AdminRequestMessage" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="AdminRequestTimeLabel" runat="server" Text="Occured at:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label ID="AdminRequestTime" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="AdminRequestTimeoutLabel" runat="server" Text="Timeout:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:Label ID="AdminRequestTimeout" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="AdminRequestOptionLabel" runat="server" Text="Option:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:RadioButtonList ID="AdminRequestOption" runat="server">
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
        <p class="FormButton">
            <asp:Button ID="ProcessAdminRequest" runat="server" Text="Process" CssClass="FormButton"
                OnClick="ProcessAdminRequest_Click" />
            <asp:Button ID="SnoozeAdminRequest" runat="server" Text="Snooze" CssClass="FormButton" />
        </p>
    </asp:Panel>
    <h3>
        Job Parameters</h3>
    <table cellpadding="0" cellspacing="0" class="Form" runat="server" id="ParametersTable">
    </table>
    <h3>
        Job Progress</h3>
    <jgwac:CheckpointProgress ID="CheckpointProgress" runat="server" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
    |
    <asp:Button ID="Cancel" runat="server" CssClass="FormButtonNarrow" Text="Cancel"
        OnCommand="Button_Command" CommandName="Cancel" />
    <asp:Button ID="Reschedule" runat="server" CssClass="FormButtonNarrow" Text="Reschedule"
        OnCommand="Button_Command" CommandName="Reschedule" />
    <asp:Button ID="Suspend" runat="server" CssClass="FormButtonNarrow" Text="Suspend"
        OnCommand="Button_Command" CommandName="Suspend" />
    <asp:Button ID="Resume" runat="server" CssClass="FormButtonNarrow" Text="Resume"
        OnCommand="Button_Command" CommandName="Resume" />
</asp:Content>
