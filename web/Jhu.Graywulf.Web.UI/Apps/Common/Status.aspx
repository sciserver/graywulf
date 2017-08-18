<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Basic/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Common.Status" CodeBehind="Status.aspx.cs" %>

<%@ Register TagPrefix="jgwc" Namespace="Jhu.Graywulf.Web.Controls" Assembly="Jhu.Graywulf.Web.Controls" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
    <style>
        div.gw-role {
            display: flex;
            flex-wrap: wrap;
        }

        div.gw-node {
            display: inline-block;
            border: 1px solid black;
            width: 160px;
            height: 120px;
            margin-right: 8px;
            margin-bottom: 8px;
            font-size: 12px;
        }

            div.gw-node span {
                background-color: black;
                color: white;
                display: inline-block;
                width: 100%;
                text-align: center;
            }

            div.gw-node img {
                margin-left: 20px;
                margin-top: 16px;
                float: left;
            }

            div.gw-node ul {
                padding-top: 24px;
                padding-left: 100px;
            }

                div.gw-node ul li {
                    font-size: 12px;
                }

        li.success {
            color: green;
        }

        li.error {
            color: red;
        }
    </style>
</asp:Content>
<asp:Content ContentPlaceHolderID="middle" runat="Server">
    <div class="dock-top gw-list-frame-top">
        <div class="gw-list-header">
            <div class="gw-list-row">
                <span style="width: 200px">system component</span>
                <span style="width: 200px">node / service status</span>
                <span class="gw-list-span"></span>
            </div>
        </div>
    </div>
    <div class="dock-bottom gw-list-frame-bottom">
    </div>
    <div class="gw-list-frame dock-fill" style="padding: 8px">
        <%
            foreach (var role in roles)
            {
        %>
        <div class="gw-list-row">
            <span style="width: 200px; min-width: 200px"><%= role.Name %></span>
            <div class="gw-role">
                <%
                    foreach (var node in role.Nodes)
                    {
                %>
                <div class="gw-node">
                    <span><%= node.Name %></span>
                    <asp:Image runat="server" ImageUrl="~/App_Themes/Basic/Icons/server.png" />
                    <ul>
                        <%
                            foreach (var check in node.Checks)
                            {
                        %>
                        <li class="<%= check.Result.ToString().ToLower() %>">
                            <%
                                if (check is Jhu.Graywulf.Scheduler.SchedulerCheck)
                                {
                                    Response.Write("SCHDLR");
                                }
                                else if (check is Jhu.Graywulf.Check.SqlServerCheck)
                                {
                                    Response.Write("SQLSRV");
                                }
                                else if (check is Jhu.Graywulf.RemoteService.RemoteServiceCheck)
                                {
                                    Response.Write("RMTSVC");
                                }
                                else if (check is Jhu.Graywulf.Web.Check.UrlCheck)
                                {
                                    Response.Write("WEBSCV");
                                }
                            %>
                        </li>
                        <%
                            }
                        %>
                    </ul>
                </div>
                <%
                    }
                %>
            </div>
        </div>
        <%
            }
        %>
    </div>
</asp:Content>
