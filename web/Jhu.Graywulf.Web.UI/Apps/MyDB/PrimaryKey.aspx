<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.PrimaryKey" CodeBehind="PrimaryKey.aspx.cs" %>

<asp:Content ContentPlaceHolderID="middle" runat="Server">
    <jgwuc:Form runat="server" Text="Manage primary key" SkinID="PrimaryKey">
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
                            <asp:Label runat="server" ID="primaryKeyNameLabel">Primary Key Name:</asp:Label>
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
                <p>The table doesn't have a Primary Key, so you can create one.</p>
                <p>
                    <asp:RadioButton runat="server" Text="Generate a new key column automatically" /><br />
                    <asp:RadioButton runat="server" Text="Select existing column as key" />
                </p>
                <table class="FormTable">
                    <tr>
                        <td class="FormLabel">
                            <asp:Label runat="server" ID="columnListLabel">Primary Key Column:</asp:Label>
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
</asp:Content>
