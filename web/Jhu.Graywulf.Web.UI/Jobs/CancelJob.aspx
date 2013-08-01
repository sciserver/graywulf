<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CancelJob.aspx.cs" Inherits="Jhu.Graywulf.Web.UI.Jobs.CancelJob"
    MasterPageFile="~/UI.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <jgwc:Form runat="server" Text="Cancel Job" SkinID="CancelJob">
        <FormTemplate>
            <p>
                Do you really want to cancel the following jobs?</p>
            <asp:BulletedList runat="server" ID="JobList">
            </asp:BulletedList>
        </FormTemplate>
        <ButtonsTemplate>
            <asp:Button ID="Ok" runat="server" Text="Yes" OnCommand="Ok_Click" CssClass="FormButton" />
            <asp:Button ID="Cancel" runat="server" OnClick="Cancel_Click" CausesValidation="False" Text="No" CssClass="FormButton" />
        </ButtonsTemplate>
    </jgwc:Form>
</asp:Content>
