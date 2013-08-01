<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.MyDB.RenameObject" CodeBehind="RenameObject.aspx.cs" %>

<%@ Register Src="~/MyDb/Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <jgwc:Form runat="server" Text="Rename MyDB object" SkinID="RenameObject">
        <FormTemplate>
            <table class="FormTable">
                <tr>
                    <td class="FormLabel">
                        <asp:Label runat="server" ID="SchemaNameLabel">Schema name:</asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:Label ID="SchemaName" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label runat="server" ID="TableNameLabel">Object name:</asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="ObjectName" runat="server" CssClass="FormField"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Ok" runat="server" Text="OK" OnClick="Ok_Click" CssClass="FormButton" />
            &nbsp;<asp:Button ID="Cancel" runat="server" CausesValidation="False" Text="Cancel"
                OnClick="Cancel_Click" CssClass="FormButton" />
        </ButtonsTemplate>
    </jgwc:Form>
</asp:Content>
