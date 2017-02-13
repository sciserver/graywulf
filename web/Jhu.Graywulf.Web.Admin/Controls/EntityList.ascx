<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntityList.ascx.cs"
    Inherits="Jhu.Graywulf.Web.Admin.Controls.EntityList" %>
<asp:ObjectDataSource runat="server" ID="InternalDataSource" EnableViewState="true"
    EnablePaging="true" SelectMethod="SelectChildren" SelectCountMethod="CountChildren"
    StartRowIndexParameterName="from" MaximumRowsParameterName="max" EnableCaching="false"
    OnObjectCreating="InternalDataSource_ObjectCreating" TypeName="Jhu.Graywulf.Registry.WebEntityFactory"></asp:ObjectDataSource>
<div runat="server" id="buttonsDiv" class="dock-bottom">
    <p class="FormMessage">
        <asp:CustomValidator ID="ItemSelectedValidator" runat="server" ErrorMessage="No item was selected."
            OnServerValidate="ItemSelectedValidator_ServerValidate" ValidationGroup="EntityList"></asp:CustomValidator>
    </p>
    <div class="btn-toolbar">
        <div class="btn-group">
            <asp:Button ID="Create" runat="server" CssClass="btn btn-default" Text="Create" />
            <asp:Button ID="Copy" runat="server" CssClass="btn btn-default" Text="Copy" CommandName="Copy"
                OnCommand="Button_Command" ValidationGroup="EntityList" />
            <asp:Button ID="Edit" runat="server" CssClass="btn btn-default" Text="Edit" CommandName="Edit"
                OnCommand="Button_Command" ValidationGroup="EntityList" />
            <asp:Button ID="Delete" runat="server" CssClass="btn btn-default" Text="Delete"
                CommandName="Delete" OnCommand="Button_Command" ValidationGroup="EntityList" />
            <asp:Button ID="Export" runat="server" CssClass="btn btn-default" Text="Export"
                CommandName="Export" OnCommand="Button_Command" ValidationGroup="EntityList" />
        </div>
        <%--
        <div class="btn-group dropup">
            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                Deploy status
                <span class="caret"></span>
            </button>
            <ul class="dropdown-menu dropup">
                <li><asp:LinkButton runat="server" Text="Undeploy" /></li>
                <li><asp:LinkButton runat="server" Text="Deploy"/></li>
            </ul>
        </div>
        --%>
        <div class="btn-group">
            <asp:Button ID="MoveUp" runat="server" CssClass="btn btn-default" Text="▲" CommandName="MoveUp"
                OnCommand="Button_Command" ValidationGroup="EntityList" />
            <asp:Button ID="MoveDown" runat="server" CssClass="btn btn-default" Text="▼" CommandName="MoveDown"
                OnCommand="Button_Command" ValidationGroup="EntityList" />
        </div>
    </div>
</div>
<div class="dock-fill" style="overflow: auto">
    <jgwc:MultiSelectGridView runat="server" ID="InternalGridView" DataSourceID="InternalDataSource"
        AllowPaging="true" DataKeyNames="Guid" AutoGenerateColumns="false" PagerSettings-Mode="NumericFirstLast" PageSize="50">
    </jgwc:MultiSelectGridView>
</div>
