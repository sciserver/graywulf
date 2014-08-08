<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.JobInstanceDependencyForm" MasterPageFile="~/EntityForm.master"
    CodeBehind="JobInstanceDependencyForm.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="Form">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="JobInstanceLabel" runat="server" Text="Predecessor Job Instance:"></asp:Label>
                &nbsp;
            </td>
            <td class="FormField">
                <asp:DropDownList ID="PredecessorJobInstance" runat="server" CssClass="FormField">
                </asp:DropDownList>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ConditionLabel" runat="server" Text="Run only if:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:DropDownList ID="Condition" runat="server" CssClass="FormField">
                    <asp:ListItem Value="Unknown" Text="(select status)" />
                    <asp:ListItem Value="Completed" Text="Completed" />
                    <asp:ListItem Value="Failed" Text="Failed" />
                    <asp:ListItem Value="Cancelled" Text="Cancelled" />
                    <asp:ListItem Value="TimedOut" Text="TimedOut" />
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>