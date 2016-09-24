<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.DropObject" CodeBehind="DropTable.aspx.cs" %>

<%@ Register Src="Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<asp:Content ContentPlaceHolderID="middle" runat="Server">
    <jgwuc:Form runat="server" Text="Drop MyDB objects" SkinID="DropObject">
        <FormTemplate>
            <p>
                Do you want to drop the following MyDB objects?</p>
                <asp:BulletedList runat="server" ID="ObjectList"></asp:BulletedList>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Ok" runat="server" Text="OK" OnClick="Ok_Click" CssClass="FormButton" />
            &nbsp;<asp:Button ID="Cancel" runat="server" CausesValidation="False" Text="Cancel"
                OnClick="Cancel_Click" CssClass="FormButton" /></ButtonsTemplate>
    </jgwuc:Form>
</asp:Content>
