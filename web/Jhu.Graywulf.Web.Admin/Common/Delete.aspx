<%@ Page Language="C#"  Inherits="Jhu.Graywulf.Web.Admin.Common.Delete" MasterPageFile="~/Admin.master"
    CodeBehind="Delete.aspx.cs" %>


<asp:Content runat="server" ContentPlaceHolderID="middle">
    <h3>
        Deleting Item</h3>
    <p>
        The following item will be deleted: '<asp:Label ID="Name" runat="server" Text="Label"></asp:Label>'.
    </p>
    <p class="Message">
        <asp:Label ID="Message" runat="server" Text="Label" Visible="False"></asp:Label></p>
    <p class="FormButton">
        <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButtonRed" OnClick="Ok_Click" />
        <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" OnClick="Cancel_Click"
            CausesValidation="False" />
    </p>
</asp:Content>
