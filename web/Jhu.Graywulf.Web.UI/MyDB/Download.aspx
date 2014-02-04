<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="True"
    Inherits="Jhu.Graywulf.Web.UI.MyDB.Download" CodeBehind="Download.aspx.cs" %>

<%@ Register Src="~/MyDb/Tabs.ascx" TagPrefix="jgwc" TagName="MyDbTabs" %>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <div class="dock-top">
        <jgwc:MyDbTabs ID="MyDbTabs1" runat="server" SelectedTab="Download" />
    </div>
    <div class="dock-bottom">
        <p class="FormMessage">
            <asp:CustomValidator ID="JobSelectedValidator" runat="server" ErrorMessage="No item was selected."
                OnServerValidate="JobSelected_ServerValidate" />
            <asp:CustomValidator ID="JobNotCompleteValidator" runat="server" ErrorMessage="File cannot be downloaded because the export job has not completed." /></p>
        <p class="FormButtons">
            <asp:Button ID="DownloadFile" runat="server" Text="Download" CssClass="FormButton"
                CommandName="Download" OnCommand="Button_Command" />
            |
            <asp:Button ID="View" runat="server" Text="View Details" CssClass="FormButton" CommandName="View"
                OnCommand="Button_Command" />
            <asp:Button ID="Cancel" runat="server" Text="Cancel Job" CssClass="FormButton" CommandName="Cancel"
                OnCommand="Button_Command" />
        </p>
    </div>
    <div class="TabFrame dock-fill">
        <asp:ObjectDataSource runat="server" ID="jobDataSource" EnableViewState="true" EnablePaging="true"
            OnObjectCreating="jobDataSource_ObjectCreating" SelectCountMethod="CountJobs"
            SelectMethod="SelectJobs" TypeName="Jhu.Graywulf.Web.Api.JobFactory" StartRowIndexParameterName="from"
            MaximumRowsParameterName="max" EnableCaching="false" />
        <jgwc:MultiSelectGridView runat="server" ID="JobList" DataSourceID="jobDataSource"
            AllowPaging="true" AutoGenerateColumns="false" SelectionMode="Multiple" DataKeyNames="Guid">
            <Columns>
                <jgwc:SelectionField ItemStyle-Width="24px" />
                <jgwc:BoundField DataField="Name" HeaderText="Job ID" ItemStyle-Width="150px" />
                <jgwc:BoundDateTimeField DataField="DateCreated" HeaderText="Submitted" ItemStyle-Width="130px"
                    ItemStyle-HorizontalAlign="Center" />
                <jgwc:BoundDateTimeField DataField="DateStarted" HeaderText="Started" ItemStyle-Width="130px"
                    ItemStyle-HorizontalAlign="Center" />
                <jgwc:BoundDateTimeField DataField="DateFinished" HeaderText="Finished" ItemStyle-Width="130px"
                    ItemStyle-HorizontalAlign="Center" />
                <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <jgwuc:JobStatus runat="server" Status='<%# (Jhu.Graywulf.Web.Api.JobStatus)Eval("Status") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <jgwc:BoundField DataField="Comments" HeaderText="Comments" ItemStyle-CssClass="GridViewSpan"
                    HeaderStyle-CssClass="GridViewSpan" />
            </Columns>
        </jgwc:MultiSelectGridView>
    </div>
</asp:Content>
