using System;
using System.Linq;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Jobs.ExportTables;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;

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
            MyDBDataset.Tables.LoadAll();
            MyDBDataset.Views.LoadAll();

            TableName.DataSource = MyDBDataset.Tables.Values.OrderBy(t => t.UniqueKey);
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
            var dfs = FileFormatFactory.GetFileFormatDescriptions();

            foreach (var df in dfs)
            {
                if (df.Value.CanWrite)
                {
                    var li = new ListItem(df.Value.DisplayName, df.Key);
                    FileFormat.Items.Add(li);
                }
            }
        }

        private void ScheduleExportTableJob()
        {
            var table = (Jhu.Graywulf.Schema.Table)SchemaManager.GetDatabaseObjectByKey(TableName.SelectedValue);
            var format = FileFormatFactory.GetFileFormatDescription(FileFormat.SelectedValue);

            // Make sure it's in MYDB
            if (StringComparer.InvariantCultureIgnoreCase.Compare(table.DatasetName, MyDBDatabaseDefinition.Name) != 0)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            var queue = EntityFactory.CombineName(EntityType.QueueInstance, Federation.ControllerMachine.GetFullyQualifiedName(), Jhu.Graywulf.Registry.Constants.LongQueueName);
            var f = new Jhu.Graywulf.Jobs.ExportTables.ExportTablesFactory(RegistryContext);

            // TODO: maybe add comments?
            var job = f.ScheduleAsJob(Federation, new[] { table }, null, format, queue, "");

            job.Save();
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