using System;
using System.Linq;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Jobs.ExportTable;
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

            TableName.DataSource = MyDBDataset.Tables.Values.OrderBy(t => t.ObjectKey);
            TableName.DataBind();

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
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
            var dfs = FileFormatFactory.Create().GetFileFormatDescriptions();

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
            var format = FileFormatFactory.Create().GetFileFormatDescription(FileFormat.SelectedValue);

            // Make sure it's in MYDB
            if (StringComparer.InvariantCultureIgnoreCase.Compare(table.DatasetName, MyDBDatabaseDefinition.Name) != 0)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            var queue = String.Format("{0}.{1}", Federation.ControllerMachine.GetFullyQualifiedName(), Jhu.Graywulf.Registry.Constants.LongQueueName);
            var f = new Jhu.Graywulf.Jobs.ExportTable.ExportTablesFactory(RegistryContext);
            
            var job = f.ScheduleAsJob(
                table,
                Jhu.Graywulf.Web.UI.AppSettings.ExportDir,
                format,
                queue,
                "");

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