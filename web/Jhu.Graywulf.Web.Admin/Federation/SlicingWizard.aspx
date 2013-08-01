<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Federation.SlicingWizard"
    MasterPageFile="~/Admin.master" CodeBehind="SlicingWizard.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <jgwac:EntityPath ID="path" runat="server" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-bottom">
        <p class="Message">
            <asp:Label ID="Message" runat="server" Text="Label" Visible="False"></asp:Label>
        </p>
        <p class="FormButton">
            <asp:Button ID="Ok" runat="server" CssClass="FormButton" OnClick="Ok_Click" Text="OK" />
            <asp:Button ID="Cancel" runat="server" CssClass="FormButton" Text="Cancel" OnClick="Cancel_Click"
                CausesValidation="False" />
        </p>
    </div>
    <div class="dock-fill">
        <h3>
            Slicing Wizard:
            <asp:Label ID="Name" runat="server" Text="Label"></asp:Label></h3>
        <table cellpadding="0" cellspacing="0" class="DetailsForm">
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="SliceCountLabel" runat="server" Text="Slice Count:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:TextBox ID="SliceCount2" runat="server" CssClass="FormField" AutoPostBack="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="PartitionCountLabel" runat="server" Text="Partition Count:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:TextBox ID="PartitionCount2" runat="server" CssClass="FormField" AutoPostBack="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="FormLabel">
                    <asp:Label ID="PartitionRangeTypeLabel" runat="server" Text="Range Type:"></asp:Label>
                </td>
                <td class="FormField">
                    <asp:DropDownList ID="PartitionRangeType" runat="server" CssClass="FormFieldNarrow">
                        <asp:ListItem>Left</asp:ListItem>
                        <asp:ListItem>Right</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <h3>
            Set Slice and Partition limits:</h3>
        <asp:Table runat="server" ID="LimitsTable">
            <asp:TableHeaderRow runat="server">
                <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">Name</asp:TableHeaderCell><asp:TableHeaderCell
                    runat="server">Name</asp:TableHeaderCell><asp:TableHeaderCell runat="server">Limit</asp:TableHeaderCell></asp:TableHeaderRow>
        </asp:Table>
    </div>
</asp:Content>
