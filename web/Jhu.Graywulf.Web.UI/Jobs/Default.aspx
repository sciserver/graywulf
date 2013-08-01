<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Jobs.Default" CodeBehind="Default.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <meta http-equiv="Refresh" content="60">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <asp:ObjectDataSource runat="server" ID="jobDataSource" EnableViewState="true" EnablePaging="true"
        OnObjectCreating="jobDataSource_ObjectCreating" SelectCountMethod="CountJobs"
        SelectMethod="SelectJobs" TypeName="Jhu.Graywulf.Web.JobDescriptionFactory" StartRowIndexParameterName="from"
        MaximumRowsParameterName="max" EnableCaching="false" />
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <div class="dock-top">
                <jgwc:MultiViewTabHeader runat="server" MultiViewID="multiView" />
            </div>
            <div class="dock-bottom">
                <p class="FormMessage">
                    <asp:CustomValidator ID="JobSelectedValidator" runat="server" ErrorMessage="No job was selected."
                        OnServerValidate="JobSelected_ServerValidate"></asp:CustomValidator></p>
                <p class="FormButtons">
                    <asp:Button ID="Details" runat="server" Text="View Details" CssClass="FormButton"
                        CommandName="Details" OnCommand="Button_Command" />
                    <asp:Button ID="Cancel" runat="server" Text="Cancel Jobs" CssClass="FormButton" CommandName="Cancel"
                        OnCommand="Button_Command" />
                </p>
            </div>
            <div class="TabFrame dock-fill dock-scroll">
                <asp:MultiView runat="server" ID="multiView" ActiveViewIndex="0">
                    <jgwc:TabView runat="server" Text="All Jobs">
                        <jgwc:MultiSelectGridView runat="server" ID="JobList" DataSourceID="jobDataSource"
                            AllowPaging="true" PageSize="30" AutoGenerateColumns="false" SelectionMode="Multiple" DataKeyNames="Guid">
                            <Columns>
                                <jgwc:SelectionField ItemStyle-Width="24px" />
                                <jgwc:BoundField DataField="Job.Name" HeaderText="Job ID" ItemStyle-Width="150px" />
                                <asp:BoundField DataField="JobType" HeaderText="Job type" ItemStyle-Width="120px" />
                                <jgwc:BoundDateTimeField DataField="Job.DateCreated" HeaderText="Submitted" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="Job.DateStarted" HeaderText="Started" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="Job.DateFinished" HeaderText="Finished" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <jgwc:JobStatus runat="server" Status='<%# (Jhu.Graywulf.Registry.JobExecutionState)Eval("Job.JobExecutionStatus") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <jgwc:BoundField DataField="Job.Comments" HeaderText="Comments" ItemStyle-CssClass="GridViewSpan"
                                    HeaderStyle-CssClass="GridViewSpan" />
                            </Columns>
                        </jgwc:MultiSelectGridView>
                    </jgwc:TabView>
                    <jgwc:TabView runat="server" Text="Query Jobs">
                        <jgwc:MultiSelectGridView runat="server" ID="QueryJobList" DataSourceID="jobDataSource"
                            AllowPaging="true" PageSize="30" AutoGenerateColumns="false" SelectionMode="Multiple" DataKeyNames="Guid">
                            <Columns>
                                <jgwc:SelectionField ItemStyle-Width="24px" />
                                <jgwc:BoundField DataField="Job.Name" HeaderText="Job ID" ItemStyle-Width="150px" />
                                <asp:TemplateField HeaderText="Output table" ItemStyle-Width="120px">
                                    <ItemTemplate>
                                        <%# Eval("SchemaName") %>.<%# Eval("ObjectName") %></ItemTemplate>
                                </asp:TemplateField>
                                <jgwc:BoundDateTimeField DataField="Job.DateCreated" HeaderText="Submitted" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="Job.DateStarted" HeaderText="Started" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="Job.DateFinished" HeaderText="Finished" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <jgwc:JobStatus runat="server" Status='<%# (Jhu.Graywulf.Registry.JobExecutionState)Eval("Job.JobExecutionStatus") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <jgwc:BoundField DataField="Job.Comments" HeaderText="Comments" ItemStyle-CssClass="GridViewSpan"
                                    HeaderStyle-CssClass="GridViewSpan" />
                            </Columns>
                        </jgwc:MultiSelectGridView>
                    </jgwc:TabView>
                    <jgwc:TabView runat="server" Text="Export Jobs">
                        <jgwc:MultiSelectGridView runat="server" ID="ExportJobList" DataSourceID="jobDataSource"
                            AllowPaging="true" PageSize="30" AutoGenerateColumns="false" SelectionMode="Multiple" DataKeyNames="Guid">
                            <Columns>
                                <jgwc:SelectionField ItemStyle-Width="24px" />
                                <jgwc:BoundField DataField="Job.Name" HeaderText="Job ID" ItemStyle-Width="150px" />
                                <asp:TemplateField HeaderText="Table name" ItemStyle-Width="120px">
                                    <ItemTemplate>
                                        <%# Eval("SchemaName") %>.<%# Eval("ObjectName") %></ItemTemplate>
                                </asp:TemplateField>
                                <jgwc:BoundDateTimeField DataField="Job.DateCreated" HeaderText="Submitted" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="Job.DateStarted" HeaderText="Started" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="Job.DateFinished" HeaderText="Finished" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <jgwc:JobStatus ID="JobStatus1" runat="server" Status='<%# (Jhu.Graywulf.Registry.JobExecutionState)Eval("Job.JobExecutionStatus") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <jgwc:BoundField DataField="Job.Comments" HeaderText="Comments" ItemStyle-CssClass="GridViewSpan"
                                    HeaderStyle-CssClass="GridViewSpan" />
                            </Columns>
                        </jgwc:MultiSelectGridView>
                    </jgwc:TabView>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
