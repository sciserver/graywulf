<%@ Page Language="C#" AutoEventWireup="true" Inherits="Jhu.Graywulf.Web.UI.Query.Results" Codebehind="Results.aspx.cs" %>
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
            margin: 5px;
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

    <asp:Literal runat="server" ID="dataTable" />

    </div>
    </form>
</body>
</html>
