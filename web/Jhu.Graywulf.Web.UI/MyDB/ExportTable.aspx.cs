using System;
using System.Linq;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Jobs.ExportTables;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Web.UI.Api;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class ExportTable : PageBase
    {
        public static string GetUrl()
        {
            return "~/MyDb/ExportTable.aspx";
        }

        public static string GetUrl(string objid)
        {
            return String.Format("~/MyDb/ExportTable.aspx?objid={0}", objid);
        }

        protected override void OnInit(EventArgs e)
        {
            FederationContext.MyDBDataset.Tables.LoadAll();
            FederationContext.MyDBDataset.Views.LoadAll();

            TableName.DataSource = FederationContext.MyDBDataset.Tables.Values.OrderBy(t => t.UniqueKey);
            TableName.DataBind();

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                DownloadLink.NavigateUrl = Download.GetUrl();

                RefreshFileFormatList();

                string objid = Request.QueryString["objid"];
                if (objid != null)
                {
                    TableName.SelectedValue = objid;
                }
            }

            base.OnLoad(e);
        }

        private void RefreshFileFormatList()
        {
            var dfs = FederationContext.FileFormatFactory.GetFileFormatDescriptions();

            foreach (var df in dfs)
            {
                if (df.Value.CanWrite)
                {
                    var li = new ListItem(df.Value.DisplayName, df.Value.DefaultExtension);
                    FileFormat.Items.Add(li);
                }
            }
        }

        private void ScheduleExportTableJob()
        {
            var ef = ExportTablesFactory.Create(FederationContext.Federation);
            var settings = ef.GetJobDefinitionSettings();

            var path = settings.OutputDirectory;
            //path = Path.Combine(path, String.Format("{0}_{1}{2}", RegistryContext.UserName, job.JobID, Jhu.Graywulf.IO.Constants.FileExtensionZip));

            var ej = new ExportJob()
            {
                Tables = new string[] { TableName.SelectedValue },
                Format = FileFormat.SelectedValue,
                Uri = path,
                Queue = JobQueue.Long,
                Comments = "",
            };

            ej.Schedule(FederationContext);
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            ScheduleExportTableJob();
            Response.Redirect(Jhu.Graywulf.Web.UI.Jobs.Default.GetUrl());
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer);
        }
    }
}