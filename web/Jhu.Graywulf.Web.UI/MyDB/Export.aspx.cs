using System;
using System.Linq;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
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

        #region Private member variables

        private Dictionary<string, Control> exportForms;

        #endregion

        public Export()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.exportForms = new Dictionary<string, Control>();
        }

        #region Event handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            CreateExportMethodForms();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DownloadLink.NavigateUrl = ExportList.GetUrl();

                RefreshExportMethodList();

                /* TODO:
                string objid = Request.QueryString["objid"];
                if (objid != null)
                {
                    TableName.SelectedValue = objid;
                }
                 * */
            }
        }

        protected void ExportMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (exportMethod.SelectedValue == "download")
            {
                // Set all plugin forms invisible
                foreach (var control in exportForms)
                {
                    control.Value.Visible = false;
                }
            }
            else
            {
                foreach (var control in exportForms)
                {
                    control.Value.Visible = StringComparer.InvariantCultureIgnoreCase.Compare(control.Key, exportMethod.SelectedValue) == 0;
                }
            }
        }

        protected void ToggleAdvanced_Click(object sender, EventArgs e)
        {
            detailsPanel.Visible = !detailsPanel.Visible;

            if (detailsPanel.Visible)
            {
                toggleAdvanced.Text = "simple mode";
            }
            else
            {
                toggleAdvanced.Text = "advanced mode";
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                if (exportMethod.SelectedValue == "download")
                {
                    // TODO
                    //ImportUploadedFile();
                    //uploadResultsForm.Visible = true;
                }
                else
                {
                    ScheduleExportJob();
                    jobResultsForm.Visible = true;
                }

                exportForm.Visible = false;
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer);
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.UI.MyDB.Tables.GetUrl());
        }

        #endregion

        private void CreateExportMethodForms()
        {
            var factory = ExportTablesJobFactory.Create(RegistryContext.Federation);

            foreach (var method in factory.EnumerateMethods())
            {
                var control = LoadControl(method.GetForm());

                control.Visible = false;
                exportFormPlaceholder.Controls.Add(control);
                exportForms.Add(method.ID, control);
            }
        }

        private void RefreshExportMethodList()
        {
            var factory = ExportTablesJobFactory.Create(RegistryContext.Federation);

            foreach (var method in factory.EnumerateMethods())
            {
                exportMethod.Items.Add(new ListItem(method.Description, method.ID));
            }
        }
        
        private void ScheduleExportJob()
        {
            var form = (IExportTablesForm)exportForms[exportMethod.SelectedValue];

            var uri = form.Uri;
            var credentials = form.Credentials;
            var file = fileFormatForm.GetFormat();
            var table = sourceTableForm.GetTable();

            var job = new ExportJob()
            {
                Uri = uri,
                Credentials = credentials == null ? null : new Web.Api.V1.Credentials(credentials),
                //Source = ...
                //FileFormat = 

                Comments = commentsForm.Comments,
                Queue = JobQueue.Long,
            };

            job.Schedule(FederationContext);

            /* TODO: delete
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
            */
        }
    }
}