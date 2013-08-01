<%@ Control Language="C#"  Inherits="Jhu.Graywulf.Web.Admin.Log.LogEventDetails"
    CodeBehind="LogEventDetails.ascx.cs" %>

<%@ Register TagPrefix="jgwc" TagName="EntityLink" Src="~/Controls/EntityLink.ascx" %>
<style type="text/css">
    .style1
    {
        width: 100%;
    }
</style>
<table cellpadding="0" cellspacing="0" class="style1">
    <tr>
        <td>
            <asp:Label ID="EntityLabel" runat="server" Text="Entity:"></asp:Label>
        </td>
        <td>
            <jgwc:EntityLink ID="EntityLink" runat="server" />
        </td>
        <td>
            <asp:Label ID="EntityFromLabel" runat="server" Text="Entity From:"></asp:Label>
        </td>
        <td>
            <jgwc:EntityLink ID="EntityFromLink" runat="server" />
        </td>
        <td>
            <asp:Label ID="EntityToLabel" runat="server" Text="Entity To:"></asp:Label>
        </td>
        <td>
            <jgwc:EntityLink ID="EntityToLink" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
        <td>
            &nbsp;
        </td>
    </tr>
</table>
