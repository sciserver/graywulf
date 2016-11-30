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
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Export : FederationPageBase
    {
        public static string GetUrl()
        {
            return "~/Apps/MyDb/Export.aspx";
        }

        public static string GetUrl(string objid)
        {
            return String.Format("~/Apps/MyDb/Export.aspx?objid={0}", objid);
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
                //RefreshDatasetList(sourceTableForm.DatasetList);
                //sourceTableForm.RefreshTableList();
            }

            if (!IsPostBack)
            {
                RefreshExportMethodList();
            }
        }

        protected void ExportMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (exportMethod.SelectedValue == "download")
            {
                commentsForm.Visible = false;

                // Set all plugin forms invisible
                foreach (var control in exportForms)
                {
                    control.Value.Visible = false;
                }
            }
            else
            {
                commentsForm.Visible = true;

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
                    ExportViaBrowser();
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
            Response.Redirect(OriginalReferer, false);
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.UI.Apps.MyDB.Tables.GetUrl(), false);
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

        private void ExportViaBrowser()
        {
            // TODO: add support for multi-table downloads

            var compression = compressionForm.Visible ? compressionForm.Compression : DataFileCompression.GZip;
            var file = fileFormatForm.GetDataFile();
            var table = sourceTableForm.Table;
            
            var uri = new Uri(table.ObjectName + file.Description.Extension, UriKind.RelativeOrAbsolute);
            uri = FederationContext.StreamFactory.AppendCompressionExtension(uri, compression);
            file.Uri = uri;
            file.Compression = compression;

            var task = new ExportTable()
            {
                BatchName = table.ObjectName,
                Source = SourceTableQuery.Create(table),
                Destination = file,
                StreamFactoryType = RegistryContext.Federation.StreamFactory,
                FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
            };

            // Set response headers
            Response.BufferOutput = false;

            if (compression != DataFileCompression.None)
            {
                Response.ContentType = Jhu.Graywulf.IO.Constants.CompressionMimeTypes[compression];
            }
            else
            {
                Response.ContentType = file.Description.MimeType;
            }

            Response.AppendHeader("Content-Disposition", "attachment; filename=" + uri.ToString());

            // Run export
            var sf = FederationContext.StreamFactory;
            using (var stream = sf.Open(Response.OutputStream, DataFileMode.Write, compression, DataFileArchival.None))
            {
                file.Open(stream, DataFileMode.Write);
                task.Execute();
                stream.Flush();
            }

            Response.End();
        }
        
        private void ScheduleExportJob()
        {
            var form = (IExportTablesForm)exportForms[exportMethod.SelectedValue];

            var uri = form.Uri;
            var credentials = form.Credentials;
            var format = fileFormatForm.GetFormat();
            var compression = compressionForm.Compression;
            var table = sourceTableForm.Table;

            // Append compression extension, if necessary
            uri = FederationContext.StreamFactory.AppendCompressionExtension(uri, compression);

            var job = new ExportJob()
            {
                Uri = uri,
                Credentials = credentials == null ? null : new Web.Api.V1.Credentials(credentials),
                Source = new SourceTable()
                {
                    Dataset = table.Dataset.Name,
                    Table = table.ObjectNameWithSchema
                },
                FileFormat = format,

                Comments = commentsForm.Comments,
                Queue = JobQueue.Long,
            };

            job.Schedule(FederationContext);
        }
    }
}