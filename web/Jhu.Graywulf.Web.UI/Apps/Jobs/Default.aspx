<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.Default" CodeBehind="Default.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <meta http-equiv="Refresh" content="60" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <asp:ObjectDataSource runat="server" ID="JobDataSource" EnableViewState="true" EnablePaging="true"
        OnObjectCreating="JobDataSource_ObjectCreating" SelectCountMethod="CountJobs"
        SelectMethod="SelectJobs" TypeName="Jhu.Graywulf.Web.Api.V1.JobFactory" StartRowIndexParameterName="from"
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
                        <jgwc:MultiSelectGridView runat="server" ID="JobList" PagerSettings-Position="TopAndBottom"
                            AllowPaging="true" PageSize="30" AutoGenerateColumns="false" SelectionMode="Multiple" DataKeyNames="Guid">
                            <Columns>
                                <jgwc:SelectionField ItemStyle-Width="24px" />
                                <jgwc:BoundField DataField="Name" HeaderText="Job ID" ItemStyle-Width="150px" />
                                <jgwc:BoundField DataField="Type" HeaderText="Type" ItemStyle-Width="48px" />
                                <jgwc:BoundDateTimeField DataField="DateCreated" HeaderText="Submitted" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="DateStarted" HeaderText="Started" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="DateFinished" HeaderText="Finished" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <jgwuc:JobStatus runat="server" Status='<%# (Jhu.Graywulf.Web.Api.V1.JobStatus)Eval("Status") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <jgwc:BoundField DataField="Comments" HeaderText="Comments" ItemStyle-CssClass="GridViewSpan"
                                    HeaderStyle-CssClass="GridViewSpan" />
                            </Columns>
                        </jgwc:MultiSelectGridView>
                    </jgwc:TabView>
                    <jgwc:TabView runat="server" Text="Query Jobs">
                        <jgwc:MultiSelectGridView runat="server" ID="QueryJobList" PagerSettings-Position="TopAndBottom"
                            AllowPaging="true" PageSize="30" AutoGenerateColumns="false" SelectionMode="Multiple" DataKeyNames="Guid">
                            <Columns>
                                <jgwc:SelectionField ItemStyle-Width="24px" />
                                <jgwc:BoundField DataField="Name" HeaderText="Job ID" ItemStyle-Width="150px" />
                                <jgwc:BoundDateTimeField DataField="DateCreated" HeaderText="Submitted" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="DateStarted" HeaderText="Started" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="DateFinished" HeaderText="Finished" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <jgwuc:JobStatus runat="server" Status='<%# (Jhu.Graywulf.Web.Api.V1.JobStatus)Eval("Status") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <jgwc:BoundField DataField="Comments" HeaderText="Comments" ItemStyle-CssClass="GridViewSpan"
                                    HeaderStyle-CssClass="GridViewSpan" />
                            </Columns>
                        </jgwc:MultiSelectGridView>
                    </jgwc:TabView>
                    <jgwc:TabView runat="server" Text="Import Jobs">
                        <jgwc:MultiSelectGridView runat="server" ID="ImportJobList" PagerSettings-Position="TopAndBottom"
                            AllowPaging="true" PageSize="30" AutoGenerateColumns="false" SelectionMode="Multiple" DataKeyNames="Guid">
                            <Columns>
                                <jgwc:SelectionField ItemStyle-Width="24px" />
                                <jgwc:BoundField DataField="Name" HeaderText="Job ID" ItemStyle-Width="150px" />
                                <jgwc:BoundDateTimeField DataField="DateCreated" HeaderText="Submitted" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="DateStarted" HeaderText="Started" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="DateFinished" HeaderText="Finished" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <jgwuc:JobStatus runat="server" Status='<%# (Jhu.Graywulf.Web.Api.V1.JobStatus)Eval("Status") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <jgwc:BoundField DataField="Uri" HeaderText="Source" ItemStyle-CssClass="GridViewSpan"
                                    HeaderStyle-CssClass="GridViewSpan" />
                                <jgwc:BoundField DataField="Comments" HeaderText="Comments" ItemStyle-CssClass="GridViewSpan"
                                    HeaderStyle-CssClass="GridViewSpan" />
                            </Columns>
                        </jgwc:MultiSelectGridView>
                    </jgwc:TabView>
                    <jgwc:TabView runat="server" Text="Export Jobs">
                        <jgwc:MultiSelectGridView runat="server" ID="ExportJobList" PagerSettings-Position="TopAndBottom"
                            AllowPaging="true" PageSize="30" AutoGenerateColumns="false" SelectionMode="Multiple" DataKeyNames="Guid">
                            <Columns>
                                <jgwc:SelectionField ItemStyle-Width="24px" />
                                <jgwc:BoundField DataField="Name" HeaderText="Job ID" ItemStyle-Width="150px" />
                                <jgwc:BoundDateTimeField DataField="DateCreated" HeaderText="Submitted" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="DateStarted" HeaderText="Started" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <jgwc:BoundDateTimeField DataField="DateFinished" HeaderText="Finished" ItemStyle-Width="140px"
                                    ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <jgwuc:JobStatus runat="server" Status='<%# (Jhu.Graywulf.Web.Api.V1.JobStatus)Eval("Status") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <jgwc:BoundField DataField="Comments" HeaderText="Comments" ItemStyle-CssClass="GridViewSpan"
                                    HeaderStyle-CssClass="GridViewSpan" />
                            </Columns>
                        </jgwc:MultiSelectGridView>
                    </jgwc:TabView>
                </asp:MultiView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
