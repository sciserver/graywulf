<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.JobInstanceDependencyDetails"
    MasterPageFile="~/EntityDetails.master" CodeBehind="JobInstanceDependencyDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
    <table class="DetailsForm">
        <tr>
            <td class="FormLabel">
                <asp:Label ID="JobInstanceLabel" runat="server" Text="Predecessor Job Instance:"></asp:Label>
            </td>
            <td class="FormField">
                &nbsp;<jgwac:EntityLink ID="JobInstance" Expression="[$Parent.Name]\[$Name]"
                    runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FormLabel">
                <asp:Label ID="ConditionLabel" runat="server" Text="Run only if:"></asp:Label>
            </td>
            <td class="FormField">
                <asp:Label ID="Condition" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
