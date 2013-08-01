<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Cluster.MachineRoleForm"
    MasterPageFile="~/EntityForm.master" CodeBehind="MachineRoleForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="MachineRoleTypeLabel" runat="server" Text="Type:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="MachineRoleType" runat="server">
                    <asp:ListItem Value="Unknown" Text="(select item)" />
                    <asp:ListItem Value="StandAlone" Text="Stand-alone node" />
                    <asp:ListItem Value="FailoverSet" Text="File-over set" />
                    <asp:ListItem Value="MirroredSet" Text="Mirrored set" />
                    <asp:ListItem Value="SlicedSet" Text="Sliced set" />
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>