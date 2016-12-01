using System;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Copy : FederationPageBase
    {
        public static string GetUrl()
        {
            return GetUrl(null);
        }

        public static string GetUrl(string objid)
        {
            var url = "~/Apps/MyDb/Copy.aspx";

            if (objid != null)
            {
                url += String.Format("?objid={0}", objid);
            }

            return url;
        }

        protected Jhu.Graywulf.Schema.DatabaseObject obj;

        protected void Page_Load(object sender, EventArgs e)
        {
        }    

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                ScheduleCopyJob();
                jobResultsForm.Visible = true;
                copyForm.Visible = false;
            }

            Response.Redirect(Tables.GetUrl(), false);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer, false);
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.UI.Apps.MyDB.Tables.GetUrl(), false);
        }

        private void ScheduleCopyJob()
        {
            var job = new CopyJob()
            {
                Source = new SourceTable()
                {
                    Dataset = sourceTable.DatasetName,
                    Table = sourceTable.Table.ObjectNameWithSchema
                },
                Destination = new DestinationTable()
                {
                    Dataset = destinationTable.DatasetName,
                    Table = destinationTable.TableName
                },
                Move = dropSourceTable.Checked,
                Comments = commentsForm.Comments,
                Queue = JobQueue.Long,
            };

            job.Schedule(FederationContext);
        }
    }
}