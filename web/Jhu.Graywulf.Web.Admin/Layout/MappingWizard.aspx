<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.MappingWizard" MasterPageFile="~/Admin.master"
    CodeBehind="MappingWizard.aspx.cs" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="toolbar">
    <jgwac:EntityPath ID="EntityPath1" runat="server" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-bottom">
        <p class="Message">
            <asp:Label ID="Message" runat="server" Text="Label" Visible="False"></asp:Label></p>
        <p class="FormButton">
            <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButton" OnClick="Ok_Click" />
            <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" OnClick="Cancel_Click" /></p>
    </div>
    <div class="dock-fill">
        <h3>
            <asp:Image ID="Icon" runat="server" />
            Mapping Wizard:
            <asp:Label ID="Name" runat="server" /></h3>
        <div class="FormFrame">
            <table cellpadding="0" cellspacing="0" class="Form">
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="LayoutTypeLabel" runat="server" Text="Layout Type:"></asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:Label ID="LayoutType" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="DatabaseVersionLabel" runat="server" Text="Database Version:"></asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:DropDownList ID="databaseVersionList" runat="server" CssClass="FormFieldNarrow"
                            AutoPostBack="true" OnSelectedIndexChanged="DatabaseVersion_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="SizeFactorLabel" runat="server" Text="Size factor:"></asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="SizeFactor" runat="server" CssClass="FormFieldNarrow">1.00</asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="NamePatternLabel" runat="server" Text="Name Pattern:"></asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="NamePattern" runat="server" CssClass="FormField"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="DatabaseNamePatternLabel" runat="server" Text="Database Name Pattern:"></asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:TextBox ID="DatabaseNamePattern" runat="server" CssClass="FormField"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        &nbsp;
                    </td>
                    <td class="FormField">
                        <asp:CheckBox ID="GenerateFileGroups" Text="Generate file groups and files" runat="server"
                            CssClass="FormField" Checked="true" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        &nbsp;
                    </td>
                    <td class="FormField">
                        <asp:CheckBox ID="SkipDuplicates" Text="Skip duplicates" runat="server"
                            CssClass="FormField" Checked="true" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="FormFrame">
            <asp:Table runat="server" ID="MappingTable">
            </asp:Table>
        </div>
    </div>
</asp:Content>
