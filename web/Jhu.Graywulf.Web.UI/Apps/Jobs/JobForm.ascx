<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.JobForm" %>

<jgwuc:Form runat="Server" Text="Job details" SkinID="JobDetails">
    <FormTemplate>
        <table class="FormTable">
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="nameLabel" runat="server" Text="Job ID:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="name" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="dateCreatedLabel" runat="server" Text="Submitted:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="dateCreated" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="dateStartedLabel" runat="server" Text="Started:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="dateStarted" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="dateFinishedLabel" runat="server" Text="Finished:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="dateFinished" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="statusLabel" runat="server" Text="Status:"></asp:Label>
                </td>
                <td>
                    <jgwuc:JobStatus runat="server" ID="status" />
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="commentsLabel" runat="server" Text="Comments:"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="comments" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </FormTemplate>
    <ButtonsTemplate>
    </ButtonsTemplate>
</jgwuc:Form>
