<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Export" CodeBehind="Export.aspx.cs" %>

<%@ Register Src="Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<%@ Register Src="FileFormatForm.ascx" TagPrefix="jgwc" TagName="FileFormatForm" %>
<%@ Register Src="CommentsForm.ascx" TagPrefix="jgwc" TagName="CommentsForm" %>
<%@ Register Src="SourceTableForm.ascx" TagPrefix="jgwc" TagName="SourceTableForm" %>
<%@ Register Src="CompressionForm.ascx" TagPrefix="jgwc" TagName="CompressionForm" %>



<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-top">
        <jgwc:MyDbTabs ID="MyDbTabs" runat="server" SelectedTab="Export" />
    </div>
    <div class="TabFrame dock-fill dock-container">
        <jgwc:Form runat="server" ID="exportForm" Text="Export tables" SkinID="ExportTable">
            <formtemplate>
                <p>
                    Export tables from MyDB into various data file formats for download.
                </p>

                <jgwc:SourceTableForm runat="server" id="sourceTableForm" />

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
            </formtemplate>
            <buttonstemplate>
                <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButton" OnClick="Ok_Click" />&nbsp;
                <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" CausesValidation="false"
                    OnClick="Cancel_Click" />
            </buttonstemplate>
        </jgwc:Form>
        <jgwc:Form ID="jobResultsForm" runat="server" Text="File export results" SkinID="ExportTable"
            Visible="false">
            <formtemplate>
                <p>
                    The file export job has been scheduled and will be executed shortly.
                </p>
            </formtemplate>
            <buttonstemplate>
                <asp:Button ID="Back" runat="server" Text="OK" OnClick="Back_Click" CssClass="FormButton" />&nbsp;
            </buttonstemplate>
        </jgwc:Form>
    </div>
</asp:Content>
