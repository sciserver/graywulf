using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Tables : FederationPageBase
    {
        public static string GetUrl()
        {
            return GetUrl(null);
        }

        public static string GetUrl(string datasetName)
        {
            var url = "~/Apps/MyDb/Tables.aspx";

            if (datasetName != null)
            {
                url += "?dataset=" + HttpContext.Current.Server.UrlEncode(datasetName);
            }

            return url;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                toolbar.RefreshDatasetList();
            }

            var userdb = toolbar.Dataset;
            userdb.Tables.LoadAll(true);

            // TODO: change this to support arbitrary sorting
            var tables = userdb.Tables.Values.OrderBy(t => t.UniqueKey).ToArray();
            
            tableList.DataSource = tables;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.DataBind();
        }

        protected void Toolbar_SelectedDatasetChanged(object sender, EventArgs e)
        {
            tableList.SelectedDataKeys.Clear();
        }
        
        protected void TableList_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem &&
                e.Item.DataItem != null)
            {
                var table = (Jhu.Graywulf.Schema.Table)e.Item.DataItem;
                var objid = table.UniqueKey;

                var schema = (HyperLink)e.Item.FindControl("schema");
                var peek = (HyperLink)e.Item.FindControl("peek");
                var export = (HyperLink)e.Item.FindControl("export");
                var rename = (HyperLink)e.Item.FindControl("rename");
                var copy = (HyperLink)e.Item.FindControl("copy");
                var primaryKey = (HyperLink)e.Item.FindControl("primaryKey");
                var drop = (HyperLink)e.Item.FindControl("drop");

                schema.NavigateUrl = Schema.ColumnsView.GetUrl(objid);
                peek.NavigateUrl = Schema.Peek.GetUrl(objid);
                export.NavigateUrl = MyDB.Export.GetUrl(objid);
                rename.NavigateUrl = MyDB.Rename.GetUrl(objid);
                copy.NavigateUrl = MyDB.Copy.GetUrl(objid);
                primaryKey.NavigateUrl = MyDB.PrimaryKey.GetUrl(objid);
                drop.NavigateUrl = MyDB.Drop.GetUrl(objid);
            }
        }
    }
}