<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Layout.MirroringWizard" MasterPageFile="~/Admin.master"
    CodeBehind="MirroringWizard.aspx.cs" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="toolbar">
    <jgwac:EntityPath ID="EntityPath1" runat="server" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-bottom">
        <p class="Message">
            <asp:Label ID="Message" runat="server" Text="Label" Visible="False"></asp:Label>
        </p>
        <p class="FormButton">
            <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButton" OnClick="Ok_Click" />
            <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" OnClick="Cancel_Click" />
        </p>
    </div>
    <div class="dock-fill dock-scroll">
        <h3>
            <asp:Image ID="Icon" runat="server" />
            Mirroring Wizard:
            <asp:Label ID="Name" runat="server" /></h3>
        <div class="FormFrame">
            <table cellpadding="0" cellspacing="0" class="Form">
                <tr>
                    <td class="FormLabel">&nbsp;
                    </td>
                    <td class="FormField">
                        <asp:CheckBox ID="CascadedCopy" Text="Cascaded mirroring" runat="server"
                            CssClass="FormField" Checked="true" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">&nbsp;
                    </td>
                    <td class="FormField">
                        <asp:CheckBox ID="SkipExistingFiles" Text="Skip existing files" runat="server"
                            CssClass="FormField" Checked="true" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">&nbsp;
                    </td>
                    <td class="FormField">
                        <asp:CheckBox ID="AttachAsReadOnly" Text="Attach as read-only" runat="server"
                            CssClass="FormField" Checked="true" />
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">&nbsp;
                    </td>
                    <td class="FormField">
                        <asp:CheckBox ID="RunCheckDb" Text="Run DBCC CHECKDB" runat="server"
                            CssClass="FormField" Checked="false" />
                    </td>
                </tr>
                <tr runat="server">
                    <td class="FormLabel">
                        <asp:Label ID="QueueInstanceLabel" runat="server" Text="Queue:"></asp:Label>
                    </td>
                    <td class="FormField">
                        <asp:DropDownList ID="QueueInstance" runat="server" CssClass="FormField">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
        <div class="FormFrame">
            <jgwac:EntityList runat="server" ID="SourceDatabases" ChildrenType="DatabaseInstance"
                EntityGroup="Layout" ButtonsVisible="False">
                <columns>
                <asp:BoundField DataField="DeploymentState" HeaderText="Deployment State" />
                <asp:BoundField DataField="RunningState" HeaderText="Running State" />
            </columns>
            </jgwac:EntityList>
            <asp:Table runat="server" ID="DestinationsTable">
            </asp:Table>
        </div>
    </div>
</asp:Content>
