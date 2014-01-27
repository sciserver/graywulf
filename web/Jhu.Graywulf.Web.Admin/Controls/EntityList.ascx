<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntityList.ascx.cs"
    Inherits="Jhu.Graywulf.Web.Admin.Controls.EntityList" %>
<asp:ObjectDataSource runat="server" ID="InternalDataSource" EnableViewState="true"
    EnablePaging="true" SelectMethod="SelectChildren" SelectCountMethod="CountChildren"
    StartRowIndexParameterName="from" MaximumRowsParameterName="max" EnableCaching="false"
    OnObjectCreating="InternalDataSource_ObjectCreating" TypeName="Jhu.Graywulf.Registry.WebEntityFactory">
</asp:ObjectDataSource>
<div class="dock-bottom">
    <p class="FormMessage">
        <asp:CustomValidator ID="ItemSelectedValidator" runat="server" ErrorMessage="No item was selected."
            OnServerValidate="ItemSelectedValidator_ServerValidate" ValidationGroup="EntityList"></asp:CustomValidator></p>
    <p class="FormButtons">
        <asp:Button ID="Create" runat="server" CssClass="FormButtonNarrow" Text="Create" />
        <asp:Button ID="Copy" runat="server" CssClass="FormButtonNarrow" Text="Copy" CommandName="Copy"
            OnCommand="Button_Command" ValidationGroup="EntityList" />
        |
        <asp:Button ID="Edit" runat="server" CssClass="FormButtonNarrow" Text="Edit" CommandName="Edit"
            OnCommand="Button_Command" ValidationGroup="EntityList" />
        <asp:Button ID="Delete" runat="server" CssClass="FormButtonNarrow" Text="Delete"
            CommandName="Delete" OnCommand="Button_Command" ValidationGroup="EntityList" />
        |
        <asp:Button ID="MoveUp" runat="server" CssClass="FormButtonIcon" Text="▲" CommandName="MoveUp"
            OnCommand="Button_Command" ValidationGroup="EntityList" />
        <asp:Button ID="MoveDown" runat="server" CssClass="FormButtonIcon" Text="▼" CommandName="MoveDown"
            OnCommand="Button_Command" ValidationGroup="EntityList" />
    </p>
</div>
<div class="dock-fill" style="overflow:auto">
    <jgwc:MultiSelectGridView runat="server" ID="InternalGridView" DataSourceID="InternalDataSource"
        AllowPaging="true" DataKeyNames="Guid" AutoGenerateColumns="false" PagerSettings-Mode="NumericFirstLast" PageSize="50">
    </jgwc:MultiSelectGridView>
</div>