using System;
using System.Collections.Generic;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class DropObject : PageBase
    {
        public static string GetUrl(string objid)
        {
            return GetUrl(new string[] { objid });
        }

        public static string GetUrl(string[] objids)
        {
            var ids = Web.Util.UrlFormatter.ArrayToUrlList(objids);
            return String.Format("~/MyDb/DropObject.aspx?objid={0}", ids);
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

                // Make sure it's in MYDB
                if (StringComparer.InvariantCultureIgnoreCase.Compare(obj.DatasetName, FederationContext.MyDBDatabaseDefinition.Name) != 0)
                {
                    throw new InvalidOperationException();  // *** TODO
                }
                
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

            Response.Redirect(Tables.GetUrl());
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer);
        }
    }
}