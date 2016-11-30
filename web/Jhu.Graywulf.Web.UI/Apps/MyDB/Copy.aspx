<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.Copy" CodeBehind="Copy.aspx.cs" %>

<%@ Register Src="Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<%@ Register Src="~/Apps/MyDB/SourceTableForm.ascx" TagPrefix="jgwc" TagName="SourceTableForm" %>
<%@ Register Src="~/Apps/MyDB/DestinationTableForm.ascx" TagPrefix="jgwc" TagName="DestinationTableForm" %>



<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <div class="dock-top">
        <jgwc:MyDbTabs ID="MyDbTabs" runat="server" SelectedTab="Copy" />
    </div>
    <div class="TabFrame dock-fill dock-container">
        <jgwuc:Form runat="server" Text="Copy table" SkinID="CopyObject">
            <FormTemplate>
                <jgwc:SourceTableForm runat="server" ID="sourceTable" />
                <table class="FormTable">
                    <tr>
                        <td class="FormLabel">&nbsp;
                        </td>
                        <td class="FormField">
                            <asp:CheckBox runat="server" ID="dropSourceTable" Text="Drop source table (move table)" />
                        </td>
                    </tr>
                </table>
                <jgwc:DestinationTableForm runat="server" ID="destinationTable" />
                <ul>
                    <li>Leave table name empty to use original name.</li>
                </ul>
            </FormTemplate>
            <ButtonsTemplate>
                <asp:Button ID="Ok" runat="server" Text="OK" OnClick="Ok_Click" CssClass="FormButton" />
                &nbsp;<asp:Button ID="Cancel" runat="server" CausesValidation="False" Text="Cancel"
                    OnClick="Cancel_Click" CssClass="FormButton" />
            </ButtonsTemplate>
        </jgwuc:Form>
    </div>
</asp:Content>
