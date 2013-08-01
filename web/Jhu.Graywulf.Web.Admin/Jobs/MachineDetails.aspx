<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Jobs.MachineDetails" MasterPageFile="~/EntityChildren.master"
    CodeBehind="MachineDetails.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="FormDetails">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormTabs">
    <jgwac:EntityChildren runat="server">
        <jgwac:EntityList runat="server" ID="QueueInstanceList" ChildrenType="QueueInstance"
            EntityGroup="Jobs" />
    </jgwac:EntityChildren>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="FormButtons">
    <jgwac:EntityButtons runat="server" ID="EntityButtons" />
</asp:Content>
