<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.MyDB.ExportTable" CodeBehind="ExportTable.aspx.cs" %>

<%@ Register Src="~/MyDb/Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-top">
        <jgwc:MyDbTabs ID="MyDbTabs" runat="server" SelectedTab="Export" />
    </div>
    <div class="TabFrame dock-fill dock-container">
        <jgwc:Form runat="server" Text="Export table" SkinID="ExportTable">
            <FormTemplate>
                <p>
                    Export tables from MyDB into various data file formats for download.</p>
                <table class="FormTable">
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="TableNameLable">Table name:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:DropDownList runat="server" ID="TableName" CssClass="FormField" DataTextField="DisplayName"
                                DataValueField="UniqueKey" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="FileFormatLabel">File format:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:DropDownList runat="server" ID="FileFormat" CssClass="FormField" />
                        </td>
                    </tr>
                </table>
                <ul>
                    <li>All exported files are automatically compressed with zip compression.</li>
                    <li>Exported tables are available for <asp:HyperLink runat="server" ID="DownloadLink">download here</asp:Hyperlink>.</li>
                </ul>
            </FormTemplate>
            <ButtonsTemplate>
                <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButton" OnClick="Ok_Click" />&nbsp;
                <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" CausesValidation="false"
                    OnClick="Cancel_Click" />
            </ButtonsTemplate>
        </jgwc:Form>
    </div>
</asp:Content>
