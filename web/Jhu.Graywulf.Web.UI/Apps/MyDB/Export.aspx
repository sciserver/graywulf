<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Export" CodeBehind="Export.aspx.cs" %>

<%@ Register Src="Toolbar.ascx" TagPrefix="jgwc" TagName="Toolbar" %>

<%@ Register Src="FileFormatForm.ascx" TagPrefix="jgwc" TagName="FileFormatForm" %>
<%@ Register Src="CommentsForm.ascx" TagPrefix="jgwc" TagName="CommentsForm" %>
<%@ Register Src="SourceTableForm.ascx" TagPrefix="jgwc" TagName="SourceTableForm" %>
<%@ Register Src="CompressionForm.ascx" TagPrefix="jgwc" TagName="CompressionForm" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <jgwc:Toolbar runat="server" ID="toolbar" SelectedTab="export" DatasetVisible="false" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <jgwuc:Form runat="server" ID="exportForm" Text="Export tables" SkinID="ExportTable">
        <FormTemplate>
            <p>
                Export tables into various data file formats for download.
            </p>

            <jgwc:SourceTableForm runat="server" ID="sourceTableForm" />

            <asp:RadioButtonList runat="server" ID="exportMethod" AutoPostBack="true" OnSelectedIndexChanged="ExportMethod_SelectedIndexChanged">
                <asp:ListItem Text="Download via browser" Value="download" Selected="True" />
            </asp:RadioButtonList>

            <asp:PlaceHolder runat="server" ID="exportFormPlaceholder" />

            <jgwc:FileFormatForm runat="server" ID="fileFormatForm" FileMode="Write" Required="true" />

            <p style="text-align: center">
                <asp:LinkButton runat="server" ID="toggleAdvanced" OnClick="ToggleAdvanced_Click" CausesValidation="false">
                        advanced mode</asp:LinkButton>
            </p>

            <asp:Panel runat="server" ID="detailsPanel" Visible="false">
                <jgwc:CompressionForm runat="server" ID="compressionForm" />
                <jgwc:CommentsForm runat="server" ID="commentsForm" Visible="false" />
            </asp:Panel>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButton" OnClick="Ok_Click" />&nbsp;
                <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" CausesValidation="false"
                    OnClick="Cancel_Click" />
        </ButtonsTemplate>
    </jgwuc:Form>
    <jgwuc:Form ID="jobResultsForm" runat="server" Text="File export results" SkinID="ExportTable"
        Visible="false">
        <FormTemplate>
            <p>
                The file export job has been scheduled and will be executed shortly.
            </p>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Back" runat="server" Text="OK" OnClick="Back_Click" CssClass="FormButton" />&nbsp;
        </ButtonsTemplate>
    </jgwuc:Form>
</asp:Content>
