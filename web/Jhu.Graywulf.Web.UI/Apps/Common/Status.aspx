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
                float:left;
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
    <div class="dock-fill" style="overflow: auto;">
        <div style="max-width: 720px; margin-left: auto; margin-right: auto;">
            <h1>System status</h1>
            <%
                foreach (var role in roles)
                {
            %>
            <h2><%= role.Name %></h2>
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
            <%
                }
            %>
        </div>
    </div>
</asp:Content>
