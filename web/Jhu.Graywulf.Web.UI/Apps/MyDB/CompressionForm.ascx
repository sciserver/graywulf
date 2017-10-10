<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompressionForm.ascx.cs"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.CompressionForm" %>

<table runat="server" class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="compressionListLabel">Compression:</asp:Label>
        </td>
        <td class="FormField">
            <asp:DropDownList runat="server" ID="compressionList" CssClass="FormField" OnSelectedIndexChanged="CompressionList_SelectedIndexChanged"
                AutoPostBack="true">
                <asp:ListItem Text="none" Value="None" Selected="True" />
                <asp:ListItem Text="gzip" Value="GZip" />
                <asp:ListItem Text="bzip2" Value="BZip2" />
                <asp:ListItem Text="zip" Value="Zip" />
            </asp:DropDownList>
        </td>
    </tr>
</table>
