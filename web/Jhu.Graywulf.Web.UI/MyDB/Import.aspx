<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.MyDB.Import" CodeBehind="Import.aspx.cs" %>

<%@ Register Src="~/MyDb/Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-top">
        <jgwc:MyDbTabs ID="MyDbTabs" runat="server" SelectedTab="Upload" />
    </div>
    <div class="TabFrame dock-fill dock-container">
        <jgwc:Form runat="server" ID="ImportForm" Text="Import data files" SkinID="ImportTable">
            <FormTemplate>
                <p>
                    Import single files or archives from any location.</p>
                <ul>
                    <li>Use batch import to import large files.</li>
                    <li>Files must be accessible over the internet, hosted on a web server, FTP server etc.</li>
                    <li>File format is inferred automatically from file extension.</li>
                    <li>Table names are generated from file names.</li>
                    <li>Supported file formats:
                        <asp:BulletedList runat="server" ID="SupportedFormatsList" />
                    </li>
                    <li>When importing text files to a new table, column names will be taken from the first
                        line of the uploaded file and column types will be infered from the first 100 lines
                        automatically.</li>
                    <li>Use upload for small files.</li>
                </ul>
                <table class="FormTable">
                    <tr>
                        <td class="FormLabel" style="width: 50px">
                            <asp:Label runat="server" ID="UriLabel">URI:</asp:Label>&nbsp;&nbsp;
                        </td>
                        <td class="FormField" style="width: 420px">
                            <asp:TextBox runat="server" ID="Uri" CssClass="FormField" Width="420px" />
                        </td>
                    </tr>
                </table>
                <p style="text-align: center">
                    <asp:LinkButton runat="server" ID="ToggleAdvanced" OnClick="ToggleAdvanced_Click">advanced mode</asp:LinkButton>
                </p>
                <p>
                    To fetch data from a remote source you may need to specify the credentials:</p>
                <table runat="server" id="DetailsTable" class="FormTable" visible="false">
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="UsernameLabel">User name:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:TextBox ID="Username" runat="server" CssClass="FormField" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="PasswordLabel">Password:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:TextBox ID="Password" runat="server" CssClass="FormField" TextMode="Password" />
                        </td>
                    </tr>
                </table>
                <table runat="server" id="DetailsTable2" class="FormTable" visible="false">
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="SchemaNameLabel">Schema name:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:TextBox ID="SchemaName" runat="server" CssClass="FormField">dbo</asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="TableNamePrefixLabel">Table name prefix:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:TextBox ID="TableNamePrefix" runat="server" CssClass="FormField">Upload</asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="FileFormatLabel">File format:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:DropDownList runat="server" ID="FileFormatList" AutoPostBack="True" OnSelectedIndexChanged="FileFormat_SelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr runat="server" id="AutoDetectColumnsRow" visible="false">
                        <td class="FormLabel">
                            &nbsp;
                        </td>
                        <td class="FormField">
                            <asp:CheckBox runat="server" ID="AutoDetectColumns" Text="Detect column names automatically"
                                Checked="true" />
                        </td>
                    </tr>
                    <tr runat="server" id="GenerateIdentityRow">
                        <td class="FormLabel">
                            &nbsp;
                        </td>
                        <td class="FormField">
                            <asp:CheckBox runat="server" ID="GenerateIdentity" Text="Generate identity column"
                                Checked="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="CommentsLabel" Text="Comments:" />
                        </td>
                        <td class="FormField">
                            <asp:TextBox runat="server" ID="Comments" TextMode="MultiLine" />
                        </td>
                    </tr>
                </table>
            </FormTemplate>
            <ButtonsTemplate>
                <asp:Button ID="Ok" runat="server" Text="OK" OnClick="Ok_Click" CssClass="FormButton" />&nbsp;
                <asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click" CausesValidation="false"
                    CssClass="FormButton" />
            </ButtonsTemplate>
        </jgwc:Form>
        <jgwc:Form ID="ResultsForm" runat="server" Text="File import results" SkinID="ImportTable"
            Visible="false">
            <FormTemplate>
                <p>
                    The file import job has been scheduled and will be executed shortly.</p>
            </FormTemplate>
            <ButtonsTemplate>
                <asp:Button ID="Back" runat="server" Text="OK" OnClick="Back_Click" CssClass="FormButton" />&nbsp;
            </ButtonsTemplate>
        </jgwc:Form>
    </div>
</asp:Content>
