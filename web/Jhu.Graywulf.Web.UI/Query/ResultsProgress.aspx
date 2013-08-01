<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResultsProgress.aspx.cs"
    Inherits="Jhu.Graywulf.Web.UI.Query.ResultsProgress" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta runat="server" id="RefreshMeta" http-equiv="Refresh" content="2">
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" ID="ProgressLabel">Executing query...</asp:Label>
    </div>
    </form>
</body>
</html>
