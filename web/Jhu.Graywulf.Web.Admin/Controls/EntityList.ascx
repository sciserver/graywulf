<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntityList.ascx.cs"
    Inherits="Jhu.Graywulf.Web.Admin.Controls.EntityList" %>
<asp:ObjectDataSource runat="server" ID="InternalDataSource" EnableViewState="true"
    EnablePaging="true" SelectMethod="Find" SelectCountMethod="Count"
    StartRowIndexParameterName="from" MaximumRowsParameterName="max" SortParameterName="orderBy"
    EnableCaching="false" OnObjectCreating="InternalDataSource_ObjectCreating" TypeName="Jhu.Graywulf.Registry.EntitySearch"></asp:ObjectDataSource>
<div runat="server" id="buttonsDiv" class="dock-bottom">
    <p class="FormMessage">
        <asp:CustomValidator ID="ItemSelectedValidator" runat="server" ErrorMessage="No item was selected."
            OnServerValidate="ItemSelectedValidator_ServerValidate" ValidationGroup="EntityList"></asp:CustomValidator>
    </p>
    <nav class="btn-toolbar">
        <div class="btn-group">
            <asp:Button ID="create" runat="server" CssClass="btn btn-default" Text="New" />
            <div class="btn-group dropup" role="group">
                <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                    Deploy
                <span class="caret"></span>
                </button>
                <ul class="dropdown-menu dropup">
                    <li class="dropdown-header">Generic</li>
                    <li>
                        <asp:LinkButton runat="server" OnCommand="Button_Command" CommandName="Deploy" Text="Deploy" /></li>
                    <li>
                        <asp:LinkButton runat="server" OnCommand="Button_Command" CommandName="Undeploy" Text="Undeploy" /></li>
                    <li role="separator" class="divider"></li>
                    <li class="dropdown-header">Database</li>
                    <li>
                        <asp:LinkButton runat="server" OnCommand="Button_Command" CommandName="Allocate" Text="Allocate" /></li>
                    <li>
                        <asp:LinkButton runat="server" OnCommand="Button_Command" CommandName="Detach" Text="Detach" /></li>
                    <li>
                        <asp:LinkButton runat="server" OnCommand="Button_Command" CommandName="Attach" Text="Attach" /></li>
                    <li>
                        <asp:LinkButton runat="server" OnCommand="Button_Command" CommandName="Drop" Text="Drop" /></li>
                    <li>
                        <asp:LinkButton runat="server" OnCommand="Button_Command" CommandName="Mirror" Text="Mirror" /></li>
                </ul>
            </div>
            <div class="btn-group dropup" role="group">
                <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                    More <span class="caret"></span>
                </button>
                <ul class="dropdown-menu dropup">
                    <!--<li>
                        <asp:LinkButton ID="edit" runat="server" Text="Edit" CommandName="Edit" OnCommand="Button_Command" ValidationGroup="EntityList" /></li>-->
                    <li>
                        <asp:LinkButton ID="copy" runat="server" Text="Copy" CommandName="Copy" OnCommand="Button_Command" ValidationGroup="EntityList" /></li>
                    <li>
                        <asp:LinkButton ID="delete" runat="server" Text="Delete" CommandName="Delete" OnCommand="Button_Command" ValidationGroup="EntityList" /></li>
                    <li>
                        <asp:LinkButton ID="export" runat="server" Text="Export" CommandName="Export" OnCommand="Button_Command" ValidationGroup="EntityList" /></li>
                </ul>
            </div>
        </div>
        <div class="btn-group">
            <asp:LinkButton runat="server" OnCommand="Button_Command" CommandName="Discover" CssClass="btn btn-default"><span class="glyphicon glyphicon-refresh" aria-hidden="true" /></asp:LinkButton>
            <asp:LinkButton runat="server" OnCommand="Button_Command" CommandName="Start" CssClass="btn btn-default"><span class="glyphicon glyphicon-play" aria-hidden="true" /></asp:LinkButton>
            <asp:LinkButton runat="server" OnCommand="Button_Command" CommandName="Stop" CssClass="btn btn-default"><span class="glyphicon glyphicon-stop" aria-hidden="true" /></asp:LinkButton>
        </div>
        <div runat="server" id="moveGroup" class="btn-group">
            <asp:LinkButton ID="moveUp" runat="server" CssClass="btn btn-default" CommandName="MoveUp"
                OnCommand="Button_Command" ValidationGroup="EntityList"><span class="glyphicon glyphicon-triangle-top" aria-hidden="true" /></asp:LinkButton>
            <asp:LinkButton ID="moveDown" runat="server" CssClass="btn btn-default" CommandName="MoveDown"
                OnCommand="Button_Command" ValidationGroup="EntityList"><span class="glyphicon glyphicon-triangle-bottom" aria-hidden="true" /></asp:LinkButton>
        </div>
        <jgwac:EntityListPager runat="server" ID="EntityListPager" PagedControlID="InternalGridView" PageSize="200" class="btn-group pull-right">
            <Fields>
                <asp:NumericPagerField ButtonType="Link" ButtonCount="5" CurrentPageLabelCssClass="btn btn-default disabled" NumericButtonCssClass="btn btn-default" />
            </Fields>
        </jgwac:EntityListPager>
    </nav>
</div>
<div class="dock-fill" style="overflow: auto">
    <jgwc:MultiSelectGridView runat="server" ID="InternalGridView" DataSourceID="InternalDataSource" AllowSorting="true"
        AllowPaging="true" AllowCustomPaging="true" DataKeyNames="Guid" AutoGenerateColumns="false" PagerSettings-Visible="false">
    </jgwc:MultiSelectGridView>
</div>
