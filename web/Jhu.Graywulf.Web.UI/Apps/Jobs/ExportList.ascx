<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportList.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.Jobs.ExportList" %>

<jgwc:MultiSelectGridView runat="server" ID="list" PagerSettings-Position="TopAndBottom"
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
