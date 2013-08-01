<%@ Page Language="C#" Inherits="Jhu.Graywulf.Web.Admin.Log.Default" MasterPageFile="~/Admin.master"
    CodeBehind="Default.aspx.cs" %>

<%@ Register TagPrefix="jgwl" TagName="LogFilterControl" Src="~/Log/LogFilterControl.ascx" %>
<asp:Content runat="server" ContentPlaceHolderID="middle">
    <div class="dock-top" style="margin-top: 8px;">
        <table class="DetailsFormHeader">
            <tr>
                <td>
                    <table class="FormTitle">
                        <tr>
                            <td class="FormTitleIcon">
                                <asp:Image ID="Icon" runat="server" />
                            </td>
                            <td class="FormTitle">
                                Log browser
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="DetailsFormButton">
                    <asp:Button ID="Refresh" runat="server" Text="Refresh" CssClass="ToolButton" OnClick="Refresh_Click" />
                </td>
            </tr>
        </table>
        <div class="DetailsForm">
            <jgwl:LogFilterControl ID="LogFilterControl" runat="server" />
        </div>
    </div>
    <div class="ListViewFrame dock-fill" style="margin-top: 16px">
        <asp:ObjectDataSource runat="server" ID="eventDataSource" DataObjectTypeName="Jhu.Graywulf.Logging.Event"
            TypeName="Jhu.Graywulf.Logging.WebEventFactory" EnablePaging="true" StartRowIndexParameterName="from"
            MaximumRowsParameterName="max" SelectCountMethod="CountEvents" SelectMethod="SelectEvents"
            OnObjectCreating="eventDataSource_ObjectCreating" />
        <asp:ListView runat="server" ID="eventList" DataSourceID="eventDataSource">
            <LayoutTemplate>
                <table class="ListViewFrame">
                    <tr id="listViewHeader">
                        <td class="ListViewHeader">
                            ID
                        </td>
                        <td class="ListViewHeader">
                            #
                        </td>
                        <td class="ListViewHeader">
                            Time
                        </td>
                        <td class="ListViewHeader">
                            Context
                        </td>
                        <td class="ListViewHeader">
                            Job
                        </td>
                        <td class="ListViewHeader">
                            Source
                        </td>
                        <td class="ListViewHeader">
                            Severity
                        </td>
                        <td class="ListViewHeader">
                            Status
                        </td>
                        <td class="ListViewHeader">
                            Operation
                        </td>
                        <td class="ListViewHeader">
                            Exception
                        </td>
                    </tr>
                    <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="listViewRow">
                    <td class="ListViewCell">
                        <a href='EventDetails.aspx?eventId=<%# Eval("EventId") %>'>
                            <%# Eval("EventId") %></a>
                    </td>
                    <td class="ListViewCell">
                        <%# Eval("EventOrder") %>
                    </td>
                    <td class="ListViewCell">
                        <%# Eval("EventDateTime") %>
                    </td>
                    <td class="ListViewCell">
                        <a href='?contextGuid=<%# Eval("ContextGuid") %>'>
                            <%# Eval("ContextGuid").ToString().Substring(32) %></a>
                    </td>
                    <td class="ListViewCell">
                        <%# Eval("JobGuid").ToString().Substring(32)%>
                    </td>
                    <td class="ListViewCell">
                        <%# Eval("EventSource") %>
                    </td>
                    <td class="ListViewCell">
                        <%# Eval("EventSeverity") %>
                    </td>
                    <td class="ListViewCell">
                        <%# Eval("ExecutionStatus") %>
                    </td>
                    <td class="ListViewCell">
                        <%# Eval("Operation") %>
                    </td>
                    <td class="ListViewCell">
                        <a href='EventDetails.aspx?eventId=<%# Eval("EventId") %>'>
                            <%# Eval("ExceptionType") %></a>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
        <asp:DataPager ID="eventListPager" runat="server" PagedControlID="eventList" PageSize="20">
            <Fields>
                <asp:NextPreviousPagerField ShowNextPageButton="false" ShowLastPageButton="false"
                    ShowFirstPageButton="true" ShowPreviousPageButton="true" FirstPageText="First"
                    LastPageText="Last" NextPageText="Next" PreviousPageText="Previous" />
                <asp:NumericPagerField ButtonCount="10" NextPageText="..." PreviousPageText="..." />
                <asp:NextPreviousPagerField ShowNextPageButton="true" ShowLastPageButton="true" ShowFirstPageButton="false"
                    ShowPreviousPageButton="false" FirstPageText="First" LastPageText="Last" NextPageText="Next"
                    PreviousPageText="Previous" />
            </Fields>
        </asp:DataPager>
    </div>
</asp:Content>
