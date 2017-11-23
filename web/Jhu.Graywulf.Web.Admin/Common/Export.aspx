<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Common.Export" MasterPageFile="~/Admin.master"
    CodeBehind="Export.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="middle" class="dock-fill">
    <div class="dock-top">
    </div>
    <div class="dock-bottom">
        <p class="FormButton">
            <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButtonRed" OnClick="Ok_Click" />
            <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" OnClick="Cancel_Click"
                CausesValidation="False" />
        </p>
    </div>
    <div class="dock-fill dock-scroll">
        <h3>Export items</h3>
        list of items comes here

        <table class="DetailsForm">
            <tr>
                <td class="FormLabel">Export filter:</td>
                <td class="FormField">
                    <asp:CheckBoxList runat="server" ID="mask">
                        <asp:ListItem Value="Cluster" Text="Cluster" />
                        <asp:ListItem Value="Domain" Text="Domain" />
                        <asp:ListItem Value="Federation" Text="Federation" />
                        <asp:ListItem Value="Layout" Text="Layout" />
                        <asp:ListItem Value="Jobs" Text="Jobs" />
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr>
                <td class="FormLabel"></td>
                <td class="FormField"><asp:CheckBox runat="server" ID="recursive" Text="Recursive" Checked="true" /></td>
            </tr>
            <tr>
                <td class="FormLabel"></td>
                <td class="FormField"><asp:CheckBox runat="server" ID="excludeUserCreated" Text="Exclude user created" Checked="true" /></td>
            </tr>
        </table>
    </div>
</asp:Content>
