<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.MyDB.ImportTable" CodeBehind="ImportTable.aspx.cs" %>

<%@ Register Src="~/MyDb/Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-top">
    <jgwc:MyDbTabs ID="MyDbTabs" runat="server" SelectedTab="Import" />
    </div>
    <div class="TabFrame dock-fill dock-container">
        <jgwc:Form runat="server" Text="Import table" SkinID="ImportTable">
            <FormTemplate>
                <p>
                    Upload only files containing comma separated values (CSV).</p>
                <p>
                    When importing to a new table, column names will be taken from the first line of
                    the uploaded file and column types will be infered from the first 100 lines automatically.</p>
                <table class="FormTable">
                    <tr>
                        <td class="FormLabel" style="width: 50px">
                            <asp:Label runat="server" ID="Label1">File:</asp:Label>&nbsp;&nbsp;
                        </td>
                        <td class="FormField" style="width: 420px">
                            <input type="file" id="ImportedFile" name="ImportedFile" runat="server" class="FormField"
                                style="width: 420px" />
                        </td>
                    </tr>
                </table>
                <p style="text-align: center">
                    <asp:LinkButton runat="server" ID="ToggleDetails" OnClick="ToggleDetails_Click">advanced mode</asp:LinkButton>
                </p>
                <table runat="server" id="DetailsTable" class="FormTable" visible="false">
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
                            <asp:Label runat="server" ID="TableNameLabel">Table name:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:TextBox ID="TableName" runat="server" CssClass="FormField">import</asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="FileFormatLabel">File format:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:DropDownList runat="server" ID="FileFormat" AutoPostBack="True" OnSelectedIndexChanged="FileFormat_SelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr runat="server" id="CompressedRow" visible="false">
                        <td class="FormLabel">
                            &nbsp;
                        </td>
                        <td class="FormField">
                            <asp:CheckBox runat="server" ID="Compressed" Text="File is compressed using gzip."
                                Checked="false" />
                        </td>
                    </tr>
                    <tr runat="server" id="ColumnNamesInFirstLineRow" visible="false">
                        <td class="FormLabel">
                            &nbsp;
                        </td>
                        <td class="FormField">
                            <asp:CheckBox runat="server" ID="ColumnNamesInFirstLine" Text="Get column names from first line"
                                Checked="true" />
                        </td>
                    </tr>
                    <tr runat="server" id="GenerateIdentityRow" visible="false">
                        <td class="FormLabel">
                            &nbsp;
                        </td>
                        <td class="FormField">
                            <asp:CheckBox runat="server" ID="GenerateIdentity" Text="Generate identity column"
                                Checked="true" />
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
    </div>
</asp:Content>
