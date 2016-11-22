<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.PrimaryKey" CodeBehind="PrimaryKey.aspx.cs" %>

<asp:Content ContentPlaceHolderID="middle" runat="Server">
    <jgwuc:Form runat="server" ID="primaryKeyForm" Text="Manage primary key" SkinID="PrimaryKey">
        <FormTemplate>
            <table class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label runat="server" ID="schemaNameLabel">Schema name:</asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:Label ID="schemaName" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label runat="server" ID="objectNameLabel">Object name:</asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:Label ID="objectName" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <asp:Panel runat="server" ID="primaryKeyPanel" Visible="false">
                <p>The table currently has the following Primary Key:</p>
                <table class="FormTable">
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="primaryKeyNameLabel">Primary key name:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:Label runat="server" ID="primaryKeyName" />
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="primaryKeyColumnsLabel">Columns:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:Label runat="server" ID="primaryKeyColumns" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel runat="server" ID="createKeyPanel" Visible="false">
                <p>The table doesn't have a primary key so you can create one.</p>
                <asp:RadioButtonList runat="server" ID="primaryKeyType" RepeatLayout="Flow" RepeatDirection="Vertical"
                    AutoPostBack="true" OnSelectedIndexChanged="PrimaryKeyType_SelectedIndexChanged">
                    <asp:ListItem Value="autogen" Text="Generate a new key column automatically" Selected="True" />
                    <asp:ListItem Value="column" Text="Select existing column as key" />
                </asp:RadioButtonList>
                <table runat="server" id="columnListTable" class="FormTable" visible="false">
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="columnListLabel">Primary key column:</asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:DropDownList runat="server" ID="columnList" CssClass="FormField" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="ok" runat="server" Text="OK" OnClick="Ok_Click" CssClass="FormButton" Visible="false" />
            <asp:Button ID="dropKey" runat="server" Text="Drop Key" OnClick="DropKey_Click" CssClass="FormButton" Visible="false" />
            &nbsp;<asp:Button ID="cancel" runat="server" CausesValidation="False" Text="Cancel"
                OnClick="Cancel_Click" CssClass="FormButton" />
        </ButtonsTemplate>
    </jgwuc:Form>
    <jgwuc:Form ID="jobResultsForm" runat="server" Text="Manage primary key" SkinID="PrimaryKey"
        Visible="false">
        <FormTemplate>
            <p>
                The primary key modification job has been scheduled and will be executed shortly.
            </p>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Back" runat="server" Text="OK" OnClick="Cancel_Click" CssClass="FormButton" />&nbsp;
        </ButtonsTemplate>
    </jgwuc:Form>
</asp:Content>
