using System;
using System.Linq;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Jobs.ExportTables;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class Export : CustomPageBase
    {
        public static string GetUrl()
        {
            return "~/MyDb/Export.aspx";
        }

        public static string GetUrl(string objid)
        {
            return String.Format("~/MyDb/Export.aspx?objid={0}", objid);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                DownloadLink.NavigateUrl = ExportList.GetUrl();

                RefreshFileFormatList();
                RefreshTableList();

                string objid = Request.QueryString["objid"];
                if (objid != null)
                {
                    TableName.SelectedValue = objid;
                }
            }

            base.OnLoad(e);
        }

        private void RefreshTableList()
        {
            FederationContext.MyDBDataset.Tables.LoadAll();

            foreach (var table in FederationContext.MyDBDataset.Tables.Values.OrderBy(t => t.UniqueKey))
            {
                TableName.Items.Add(new ListItem(table.DisplayName, table.UniqueKey));
            }
        }

        private void RefreshFileFormatList()
        {
            var dfs = FederationContext.FileFormatFactory.EnumerateFileFormatDescriptions();

            foreach (var df in dfs)
            {
                if (df.CanWrite)
                {
                    var li = new ListItem(df.DisplayName, df.Extension);
                    FileFormat.Items.Add(li);
                }
            }
        }

        private void ScheduleExportTableJob()
        {
            var ef = ExportTablesJobFactory.Create(FederationContext.Federation);
            var settings = ef.GetJobDefinitionSettings();

            var path = Path.Combine(
                settings.OutputDirectory,
                String.Format(
                    "{0}_{1:yyMMddHHmmssff}{2}",
                    EntityFactory.GetName(RegistryContext.UserName),
                    DateTime.Now,
                    Jhu.Graywulf.IO.Constants.FileExtensionZip));

            // TODO: add support for multiple tables
            var tables = new string[1];
            tables[0] = FederationContext.SchemaManager.GetDatabaseObjectByKey(TableName.SelectedValue).DisplayName;

            var ej = new ExportJob()
            {
                Tables = tables,
                ContentType = FileFormat.SelectedValue,
                Uri = Jhu.Graywulf.Util.UriConverter.FromFilePath(path),
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