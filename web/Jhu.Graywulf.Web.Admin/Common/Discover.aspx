<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Common.Discover" MasterPageFile="~/Admin.master"
    CodeBehind="Discover.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-bottom">
        <p class="Message">
            <asp:Label ID="Message" runat="server" Text="Label" Visible="False"></asp:Label>
        </p>
        <p class="FormButton">
            <asp:Button ID="Ok" runat="server" Text="OK" CssClass="FormButtonRed" OnClick="Ok_Click" />
            <asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="FormButton" OnClick="Cancel_Click"
                CausesValidation="False" />
        </p>
    </div>
    <div class="dock-fill" style="overflow: scroll;">
        <h3>Discovering configuration</h3>
        <p>
            Discovery procedure ran on '<asp:Label ID="Name" runat="server" Text="Label"></asp:Label>'
            and found the following differences. Do you want to update registry to reflect these
            configuration changes?
        </p>
        <div runat="server" id="errorListDiv" visible="false">
            <h3>Errors</h3>
            <asp:BulletedList runat="server" ID="errorList" />
        </div>
        <div runat="server" id="updateListDiv" visible="true">
            <h3>Changes</h3>
            <asp:BulletedList runat="server" ID="updateList" />
        </div>
    </div>
</asp:Content>
