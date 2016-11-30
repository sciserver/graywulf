<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TableList.ascx.cs" Inherits="Jhu.Graywulf.Web.UI.Apps.MyDB.TableList" %>

<asp:DropDownList runat="server" ID="tableList"
    OnSelectedIndexChanged="TableList_SelectedIndexChanged" />