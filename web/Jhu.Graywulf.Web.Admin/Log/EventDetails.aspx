<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EventDetails.aspx.cs" MasterPageFile="~/Admin.master"
    Inherits="Jhu.Graywulf.Web.Admin.Log.EventDetails" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="middle">
    <div class="DetailsForm">
        <table class="DetailsFormHeader">
            <tr>
                <td>
                    <table class="FormTitle">
                        <tr>
                            <td class="FormTitleIcon">
                                <asp:Image ID="Icon" runat="server" />
                            </td>
                            <td class="FormTitle">
                                Log event details
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="DetailsFormButton">
                </td>
            </tr>
        </table>
        <p class="Message">
            <asp:Label ID="Label1" runat="server" Text="Label" Visible="False"></asp:Label>
        </p>
    </div>
    <jgwc:MultiViewTabHeader ID="MainTabs" runat="server" MultiViewID="MainMultiViewTabs" />
    <div class="TabFrame dock-fill">
        <asp:MultiView runat="server" ID="MainMultiViewTabs" ActiveViewIndex="0">
            <jgwc:TabView ID="tabViewMain" runat="server" Text="Details">
                <table class="DetailsForm">
                    <tr>
                        <td class="FormLabel">
                            <asp:Label ID="EventIdLabel" runat="server" Text="Event ID:"></asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:Label ID="EventId" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label ID="EventOrderLabel" runat="server" Text="Order #:"></asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:Label ID="EventOrder" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label ID="EventSeverityLabel" runat="server" Text="Severity:"></asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:Label ID="EventSeverity" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel">
                            <asp:Label ID="ExceptionTypeLabel" runat="server" Text="Exception:"></asp:Label>
                        </td>
                        <td class="FormField">
                            <asp:Label ID="ExceptionType" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </jgwc:TabView>
            <jgwc:TabView ID="tabViewSettings" runat="server" Text="Stack trace">
                <div style="font-family: Consolas">
                    <asp:Label ID="StackTrace" runat="server"></asp:Label>
                </div>
            </jgwc:TabView>
        </asp:MultiView>
    </div>
</asp:Content>
