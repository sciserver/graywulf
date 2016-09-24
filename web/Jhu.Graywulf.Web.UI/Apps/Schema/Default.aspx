<%@ Page Title="" Language="C#" MasterPageFile="~/UI.master" AutoEventWireup="true"
    Inherits="Jhu.Graywulf.Web.UI.Apps.Schema.Default" CodeBehind="Default.aspx.cs" %>

<%@ Register Src="DbObjectDetails.ascx" TagName="DbObjectDetails" TagPrefix="jgwc" %>
<asp:Content runat="server" ContentPlaceHolderID="toolbar">
    <script>
        function ShowRefreshing() {
            var datasetList = document.getElementById("<%= DatasetList.ClientID %>");
            var objectTypeList = document.getElementById("<%= ObjectTypeList.ClientID %>");
            var objectList = document.getElementById("<%= ObjectList.ClientID %>");


            datasetList.disabled = true;
            objectTypeList.disabled = true;
            objectList.disabled = true;

            objectList.options.length = 0;
            var opt = new Option("Refreshing...", "");
            objectList.options.add(opt);
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_initializeRequest(ShowRefreshing);
    </script>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <jgwc:Toolbar runat="server">
                <jgwc:ToolbarElement ID="ToolbarElement1" runat="server" Style="width: 140px">
                    <asp:Label ID="DatasetListLabel" runat="server" Text="Catalog:" /><br />
                    <asp:DropDownList ID="DatasetList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DatasetList_SelectedIndexChanged"
                        CssClass="ToolbarControl" Width="140px">
                    </asp:DropDownList>
                </jgwc:ToolbarElement>
                <jgwc:ToolbarElement ID="ToolbarElement2" runat="server" Style="width: 140px">
                    <asp:Label ID="ObjectTypeListLabel" runat="server" Text="Object category:" /><br />
                    <asp:DropDownList ID="ObjectTypeList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ObjectTypeList_SelectedIndexChanged"
                        CssClass="ToolbarControl" Width="140px">
                        <asp:ListItem Selected="True" Value="Table">Tables</asp:ListItem>
                        <asp:ListItem Value="View">Views</asp:ListItem>
                        <asp:ListItem Value="StoredProcedure">Stored Procedures</asp:ListItem>
                        <asp:ListItem Value="ScalarFunction">Scalar Functions</asp:ListItem>
                        <asp:ListItem Value="TableValuedFunction">Table-valued Functions</asp:ListItem>
                    </asp:DropDownList>
                </jgwc:ToolbarElement>
                <jgwc:ToolbarElement ID="ToolbarElement3" runat="server">
                    <asp:Label ID="ObjectListLabel" runat="server" Text="Object:" /><br />
                    <asp:DropDownList ID="ObjectList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ObjectList_SelectedIndexChanged"
                        CssClass="ToolbarControl" Style="width: 100%; box-sizing: border-box;">
                    </asp:DropDownList>
                </jgwc:ToolbarElement>
            </jgwc:Toolbar>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="middle" runat="Server">
    <asp:UpdatePanel runat="server" class="dock-fill dock-container">
        <ContentTemplate>
            <jgwc:Form runat="server" ID="IntroForm" SkinID="Schema" Text="Getting started with the schema browser">
                <FormTemplate>
                    <ul>
                        <li>Select catalog from the first list. Catalogs are equivalent to databases. MyDB
                            referers to your own user data space.</li>
                        <li>Select object type from the second list. </li>
                        <li>Select object from the third list.</li>
                    </ul>
                </FormTemplate>
                <ButtonsTemplate>
                </ButtonsTemplate>
            </jgwc:Form>
            <jgwc:DbObjectDetails runat="server" ID="DetailsPanel" Visible="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
