<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.MachineRoleDetails"
    MasterPageFile="~/EntityChildren.master" CodeBehind="MachineRoleDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="QueueInstanceList" ChildrenType="QueueInstance"
            EntityGroup="Jobs" />
        <jgwac:EntityList runat="server" ID="MachineList" ChildrenType="Machine" EntityGroup="Jobs">
        <columns>
                        <jgwuc:ExpressionPropertyField DataField="HostName" HeaderText="UNC name" />
                    </columns>
        </jgwac:EntityList>
    </jgwac:EntityChildren>
</asp:Content>

