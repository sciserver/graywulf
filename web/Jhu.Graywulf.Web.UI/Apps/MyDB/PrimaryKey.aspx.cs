using System;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class PrimaryKey : FederationPageBase
    {
        public static string GetUrl(string objid)
        {
            return String.Format("~/Apps/MyDb/PrimaryKey.aspx?objid={0}", objid);
        }

        protected Table table;

        protected void Page_Load(object sender, EventArgs e)
        {
            string objid = Request.QueryString["objid"];
            table = (Table)FederationContext.SchemaManager.GetDatabaseObjectByKey(objid);

            if (!table.Dataset.IsMutable)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (!IsPostBack)
            {
                schemaName.Text = table.SchemaName;
                objectName.Text = table.ObjectName;

                if (table.PrimaryKey != null)
                {
                    primaryKeyName.Text = table.PrimaryKey.DisplayName;
                    primaryKeyColumns.Text = table.PrimaryKey.ColumnListDisplayString;
                    primaryKeyPanel.Visible = true;
                    dropKey.Visible = true;
                }
                else
                {
                    createKeyPanel.Visible = true;
                    ok.Visible = true;
                }
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            // TODO: create PK here
            Response.Redirect(Tables.GetUrl(), false);
        }

        protected void DropKey_Click(object sender, EventArgs e)
        {

        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer, false);
        }
    }
}