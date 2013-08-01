<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.JobInstanceForm" MasterPageFile="~/EntityForm.master"
    ValidateRequest="false" CodeBehind="JobInstanceForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="Form">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="JobDefinitionLabel" runat="server" Text="Job Definition:"></asp:Label>
                &nbsp;
            </td>
            <td class="FormField">
                <asp:DropDownList ID="JobDefinition" runat="server" CssClass="FormField" AutoPostBack="True"
                    OnSelectedIndexChanged="JobDefinition_SelectedIndexChanged">
                </asp:DropDownList>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="WorkflowTypeNameLabel" runat="server" Text="Workflow Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="WorkflowTypeName" runat="server" CssClass="FormField"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="JobExecutionStatusLabel" runat="server" Text="Execution Status:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="JobExecutionStatus" runat="server" CssClass="FormFieldNarrow">
                    <asp:ListItem Value="Unknown" Text="(select status)" />
                    <asp:ListItem Value="Scheduled" Text="Scheduled" />
                    <asp:ListItem Value="Waiting" Text="Waiting" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ScheduleTypeLabel" runat="server" Text="Scheduling Method:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="ScheduleType" runat="server" CssClass="FormFieldNarrow">
                    <asp:ListItem Value="Unknown" Text="(select schedule)" />
                    <asp:ListItem Value="Queued" Text="Queued" />
                    <asp:ListItem Value="Timed" Text="Scheduled at given time" />
                    <asp:ListItem Value="Recurring" Text="Recurring" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ScheduleTimeLabel" runat="server" Text="Scheduled at:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="ScheduleTime" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="RecurringPeriodLabel" runat="server" Text="Recurring Period:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="RecurringPeriod" runat="server" CssClass="FormFieldNarrow">
                    <asp:ListItem Value="Unknown" Text="(select period)" />
                    <asp:ListItem Value="Daily" Text="Daily" />
                    <asp:ListItem Value="Weekly" Text="Weekly" />
                    <asp:ListItem Value="Monthly" Text="Monthly" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="RecurringIntervalLabel" runat="server" Text="Recurring Interval:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="RecurringInterval" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="RecurringMaskLabel" runat="server" Text="Recurring Mask:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:TextBox ID="RecurringMask" runat="server" CssClass="FormFieldNarrow"></asp:TextBox>
            </td>
        </tr>
    </table>
    <h3>
        Job Parameters</h3>
    <table cellpadding="0" cellspacing="0" class="Form" runat="server" id="ParametersTable">
    </table>
</asp:Content>