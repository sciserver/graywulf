<%@ Page Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.Peek"
    CodeBehind="Peek.aspx.cs" Theme="" %>

<%@ Import Namespace="System.Data.SqlClient" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body
        {
            font-family: Consolas, Courier New, Courier;
            font-size: 8pt;
        }
        td.header
        {
            background-color: lightgray;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <% RenderTable(); %>
    </div>
    </form>
</body>
</html>
