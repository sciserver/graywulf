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

            // Make sure it's in MYDB
            // *** TODO: implement security logic and replace this
            if (StringComparer.InvariantCultureIgnoreCase.Compare(obj.DatasetName, FederationContext.MyDBDataset.Name) != 0)
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
            obj.Rename(ObjectName.Text);
            Response.Redirect("Default.aspx", false);      // *** TODO: where to redurect after rename?
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer, false);
        }
    }
}