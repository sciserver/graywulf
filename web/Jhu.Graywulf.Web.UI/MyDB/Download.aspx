<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.MyDB.Download" CodeBehind="Download.aspx.cs" Buffer="false" %>

<%@ Register Src="~/MyDb/Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-top">
        <jgwc:MyDbTabs ID="MyDbTabs" runat="server" SelectedTab="Download" />
    </div>
    <div class="TabFrame dock-fill dock-container">
        <jgwc:Form runat="server" ID="DownloadForm" Text="Download tables" SkinID="ExportTable">
            <FormTemplate>
                <p>
                    Download tables of MyDB into various data file formats.</p>
                <ul>
                    <li>All exported files are automatically compressed with zip compression.</li>
                    <li>File names are generated from table names automatically.</li>
                    <li>Use batch export for large tables.</li>
                </ul>
                <table class="FormTable">
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="TableListLabel">Tables:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:ListBox runat="server" ID="TableList" CssClass="FormField" SelectionMode="Multiple" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="FileFormatListLabel">File format:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:DropDownList runat="server" ID="FileFormatList" CssClass="FormField" />
                        </td>
                    </tr>
                </table>
            </FormTemplate>
            <ButtonsTemplate>
                <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButton" OnClick="Ok_Click" />&nbsp;
                <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" CausesValidation="false"
                    OnClick="Cancel_Click" />
            </ButtonsTemplate>
        </jgwc:Form>
        <jgwc:Form runat="server" ID="ResultsForm" Text="Table download" SkinID="ExportTable"
            Visible="false">
            <FormTemplate>
                <p>
                    Your download is being prepared. Please <a runat="server" id="DownloadLink">click here</a>
                    if it doesn't start automatically.</p>
            </FormTemplate>
            <ButtonsTemplate>
                <asp:Button ID="Back" runat="server" Text="OK" OnClick="Back_Click" CssClass="FormButton" />&nbsp;
            </ButtonsTemplate>
        </jgwc:Form>
    </div>
</asp:Content>
