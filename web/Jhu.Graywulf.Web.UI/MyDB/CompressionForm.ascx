<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompressionForm.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.MyDB.CompressionForm" %>

<p>
    Output files can be compressed using zip:
</p>
<table runat="server" class="FormTable">
    <tr>
        <td class="FormLabel">
            <asp:Label runat="server" ID="compressionListLabel">Compression:</asp:Label>
        </td>
        <td class="FormField">
            <asp:RadioButtonList runat="server" ID="compressionList" RepeatLayout="Flow">
                <asp:ListItem runat="server" Text="none" Value="None" Selected="True" />
                <asp:ListItem runat="server" Text="gzip" Value="GZip" />
                <asp:ListItem runat="server" Text="bzip2" Value="BZip2" />
                <asp:ListItem runat="server" Text="zip" Value="Zip" />
            </asp:RadioButtonList>
        </td>
    </tr>
</table>