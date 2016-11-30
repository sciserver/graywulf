using System;
using System.Collections.Generic;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Drop : FederationPageBase
    {
        public static string GetUrl(string objid)
        {
            return GetUrl(new string[] { objid });
        }

        public static string GetUrl(string[] objids)
        {
            var ids = Util.UrlFormatter.ArrayToUrlList(objids);
            return String.Format("~/Apps/MyDb/Drop.aspx?objid={0}", ids);
        }

        private List<DatabaseObject> objs;

        protected void Page_Load(object sender, EventArgs e)
        {
            var parts = Request.QueryString["objid"].Split(',');

            objs = new List<DatabaseObject>();
            ObjectList.Items.Clear();

            foreach (var objid in parts)
            {
                var obj = FederationContext.SchemaManager.GetDatabaseObjectByKey(objid);                
                objs.Add(obj);
                ObjectList.Items.Add(obj.DisplayName);
            }
            
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            foreach (var obj in objs)
            {
                obj.Drop();
            }

            Response.Redirect(Tables.GetUrl(), false);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer, false);
        }
    }
}