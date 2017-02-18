<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Import" CodeBehind="Import.aspx.cs" %>

<%@ Register Src="Toolbar.ascx" TagPrefix="jgwc" TagName="Toolbar" %>
<%@ Register Src="UploadForm.ascx" TagPrefix="jgwc" TagName="UploadForm" %>
<%@ Register Src="FileFormatForm.ascx" TagPrefix="jgwc" TagName="FileFormatForm" %>
<%@ Register Src="DestinationTableForm.ascx" TagPrefix="jgwc" TagName="DestinationTableForm" %>
<%@ Register Src="ImportOptionsForm.ascx" TagPrefix="jgwc" TagName="ImportOptionsForm" %>
<%@ Register Src="CommentsForm.ascx" TagPrefix="jgwc" TagName="CommentsForm" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <jgwc:Toolbar runat="server" ID="toolbar" SelectedTab="import" DatasetVisible="false" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <jgwuc:Form runat="server" ID="importForm" Text="Import data files" SkinID="ImportTable">
        <FormTemplate>
            <p>
                Import single files or archives from any location.
            </p>

            <table class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label runat="server" ID="importMethodLabel">Method:</asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:RadioButtonList runat="server" ID="importMethod" AutoPostBack="true" OnSelectedIndexChanged="ImportMethod_SelectedIndexChanged"
                            CausesValidation="false">
                            <asp:ListItem Text="Upload via browser" Value="upload" Selected="True" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>

            <jgwc:UploadForm runat="server" ID="uploadForm" />
            <asp:PlaceHolder runat="server" ID="importFormPlaceholder" />

            <jgwc:DestinationTableForm runat="server" ID="destinationTableForm" />

            <div class="gw-details-container">
                <p class="FormOptionsButton">
                    <jgwc:DetailsButton runat="server" Text="more options" />
                </p>
                <div class="gw-details-panel">
                    <jgwc:FileFormatForm runat="server" ID="fileFormatForm" FileMode="Read" />
                    <jgwc:ImportOptionsForm runat="server" ID="importOptionsForm" />
                    <jgwc:CommentsForm runat="server" ID="commentsForm" />
                </div>
            </div>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Ok" runat="server" Text="OK" OnClick="Ok_Click" CssClass="FormButton" />&nbsp;
                <asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click" CausesValidation="false"
                    CssClass="FormButton" />
        </ButtonsTemplate>
    </jgwuc:Form>
    <jgwuc:Form ID="uploadResultsForm" runat="server" Text="File upload results" SkinID="ImportTable"
        Visible="false">
        <FormTemplate>
            <p>
                The following tables have been created:
            </p>
            <asp:BulletedList runat="server" ID="resultTableList" />
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Button1" runat="server" Text="OK" OnClick="Back_Click" CssClass="FormButton" />&nbsp;
        </ButtonsTemplate>
    </jgwuc:Form>
    <jgwuc:Form ID="jobResultsForm" runat="server" Text="File import results" SkinID="ImportTable"
        Visible="false">
        <FormTemplate>
            <p>
                The file import job has been scheduled and will be executed shortly.
            </p>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Back" runat="server" Text="OK" OnClick="Back_Click" CssClass="FormButton" />&nbsp;
        </ButtonsTemplate>
    </jgwuc:Form>
</asp:Content>
