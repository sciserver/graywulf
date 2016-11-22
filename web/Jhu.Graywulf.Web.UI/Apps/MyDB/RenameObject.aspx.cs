using System;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class RenameObject : FederationPageBase
    {
        public static string GetUrl(string objid)
        {
            return String.Format("~/Apps/MyDb/RenameObject.aspx?objid={0}", objid);
        }

        protected DatabaseObject obj;

        protected void Page_Load(object sender, EventArgs e)
        {
            string objid = Request.QueryString["objid"];
            obj = FederationContext.SchemaManager.GetDatabaseObjectByKey(objid);

            if (!obj.Dataset.IsMutable)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (!IsPostBack)
            {
                SchemaName.Text = obj.SchemaName;
                ObjectName.Text = obj.ObjectName;
            }
        }    

        protected void Ok_Click(object sender, EventArgs e)
        {
            obj.Rename(SchemaName.Text, ObjectName.Text);
            Response.Redirect(Tables.GetUrl(), false);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer, false);
        }
    }
}