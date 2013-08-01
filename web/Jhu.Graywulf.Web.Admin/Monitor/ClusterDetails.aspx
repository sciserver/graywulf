<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Monitor.ClusterDetails" MasterPageFile="~/Admin.master"
    CodeBehind="ClusterDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-fill" style="margin-top: 8px; overflow:scroll">
        <p class="Message">
            <asp:Label ID="Message" runat="server" Text="Label" Visible="False"></asp:Label>
        </p>
        <p>
            Diagnostic process running under <b>
                <asp:Label runat="server" ID="ProcessUser">ProcessUser</asp:Label></b></p>
        <h3>
            Machine Status:
        </h3>
        <asp:ListView ID="MessageList" runat="server" ItemPlaceholderID="ItemPlaceholder">
            <LayoutTemplate>
                <table>
                    <asp:PlaceHolder runat="server" ID="ItemPlaceholder" />
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# Eval("EntityName") %>
                    </td>
                    <td>
                        <%# Eval("NetworkName") %>
                    </td>
                    <td>
                        <%# Eval("ServiceName") %>
                    </td>
                    <td>
                        <%# Eval("Status") %>
                    </td>
                    <td>
                        <%# Eval("ErrorMessage") %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
    </div>
</asp:Content>
