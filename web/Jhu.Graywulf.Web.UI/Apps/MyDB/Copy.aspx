<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Copy" CodeBehind="Copy.aspx.cs" %>

<%@ Register Src="Toolbar.ascx" TagPrefix="jgwc" TagName="Toolbar" %>
<%@ Register Src="~/Apps/MyDB/SourceTableForm.ascx" TagPrefix="jgwc" TagName="SourceTableForm" %>
<%@ Register Src="~/Apps/MyDB/DestinationTableForm.ascx" TagPrefix="jgwc" TagName="DestinationTableForm" %>
<%@ Register Src="~/Apps/MyDB/CommentsForm.ascx" TagPrefix="jgwc" TagName="CommentsForm" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <jgwc:Toolbar runat="server" ID="toolbar" SelectedTab="copy" DatasetVisible="false" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <jgwuc:Form runat="server" ID="copyForm" Text="Copy table" SkinID="CopyTable">
                <FormTemplate>
                    <jgwc:SourceTableForm runat="server" ID="sourceTable" />
                    <table class="FormTable">
                        <tr>
                            <td class="FormLabel">&nbsp;
                            </td>
                            <td class="FormField">
                                <asp:CheckBox runat="server" ID="dropSourceTable" Text="Drop source table (move table)" />
                            </td>
                        </tr>
                    </table>
                    <jgwc:DestinationTableForm runat="server" ID="destinationTable" />
                    <ul>
                        <li>Leave table name empty to use original name.</li>
                    </ul>
                    <jgwc:CommentsForm runat="server" ID="commentsForm" />
                </FormTemplate>
                <ButtonsTemplate>
                    <asp:Button ID="Ok" runat="server" Text="OK" OnClick="Ok_Click" CssClass="FormButton" />
                    &nbsp;<asp:Button ID="Cancel" runat="server" CausesValidation="False" Text="Cancel"
                        OnClick="Cancel_Click" CssClass="FormButton" />
                </ButtonsTemplate>
            </jgwuc:Form>
            <jgwuc:Form ID="jobResultsForm" runat="server" Text="Table copy results" SkinID="CopyTable"
                Visible="false">
                <FormTemplate>
                    <p>
                        The table copy job has been scheduled and will be executed shortly.
                    </p>
                </FormTemplate>
                <ButtonsTemplate>
                    <asp:Button ID="Back" runat="server" Text="OK" OnClick="Back_Click" CssClass="FormButton" />&nbsp;
                </ButtonsTemplate>
            </jgwuc:Form>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
