<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatasetList.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.DatasetList" %>

<asp:DropDownList runat="server" ID="datasetList"
    OnSelectedIndexChanged="DatasetList_SelectedIndexChanged" />