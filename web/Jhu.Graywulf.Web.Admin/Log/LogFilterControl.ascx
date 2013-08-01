<%@ Control Language="C#"  Inherits="Jhu.Graywulf.Web.Admin.Log.LogFilterControl"
    CodeBehind="LogFilterControl.ascx.cs" %>

<%@ Register TagPrefix="jgwc" TagName="EntityLink" Src="~/Controls/EntityLink.ascx" %>
<table class="DetailsForm">
    <tr>
        <td>
            <asp:Label ID="Label1" runat="server" Text="Time:"></asp:Label>
        </td>
        <td>
            <asp:DropDownList ID="EventDateTimeRange" runat="server" AutoPostBack="True" OnSelectedIndexChanged="EventDateTimeRange_SelectedIndexChanged">
                <asp:ListItem Value="All">Any time</asp:ListItem>
                <asp:ListItem Value="Last1">Last hour</asp:ListItem>
                <asp:ListItem Value="Last12">Last 12 hours</asp:ListItem>
                <asp:ListItem Value="Last24">Last 24 hours</asp:ListItem>
                <asp:ListItem Value="Last168">Last 7 days</asp:ListItem>
                <asp:ListItem Value="Last720">Last 30 days</asp:ListItem>
                <asp:ListItem Value="Custom">Custom range</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="Label2" runat="server" Text="From:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="EventDateTimeFrom" runat="server" Enabled="False"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="Label3" runat="server" Text="To:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="EventDateTimeTo" runat="server" Enabled="False"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="Label4" runat="server" Text="Event Serverity:"></asp:Label>
        </td>
        <td>
            <asp:CheckBoxList ID="EventSeverity" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>Status</asp:ListItem>
                <asp:ListItem>Warning</asp:ListItem>
                <asp:ListItem>Error</asp:ListItem>
            </asp:CheckBoxList>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="Label5" runat="server" Text="Event Source:"></asp:Label>
        </td>
        <td>
            <asp:CheckBoxList ID="EventSource" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>Scheduler</asp:ListItem>
                <asp:ListItem>Job</asp:ListItem>
                <asp:ListItem>Workflow</asp:ListItem>
                <asp:ListItem>Registry</asp:ListItem>
                <asp:ListItem>StoredProcedure</asp:ListItem>
                <asp:ListItem>UserCode</asp:ListItem>
            </asp:CheckBoxList>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="Label6" runat="server" Text="Execution Status:"></asp:Label>
        </td>
        <td>
            <asp:CheckBoxList ID="ExecutionStatus" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>Executing</asp:ListItem>
                <asp:ListItem>Canceled</asp:ListItem>
                <asp:ListItem>Closed</asp:ListItem>
                <asp:ListItem>Compensating</asp:ListItem>
                <asp:ListItem>Faulted</asp:ListItem>
            </asp:CheckBoxList>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="Label7" runat="server" Text="Filtered by:"></asp:Label>
        </td>
        <td>
            <jgwc:EntityLink ID="linkUser" runat="server" />
            <jgwc:EntityLink ID="linkJob" runat="server" />
            <jgwc:EntityLink ID="linkEntity" runat="server" />
        </td>
    </tr>
</table>
