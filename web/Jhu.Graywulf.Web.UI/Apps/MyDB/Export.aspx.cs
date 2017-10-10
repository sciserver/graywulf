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

        protected void SourceTableForm_SelectionChanged(object sender, EventArgs e)
        {
            UpdateUri();
        }

        protected void FileFormatForm_SelectionChanged(object sender, EventArgs e)
        {
            UpdateUri();
        }

        protected void CompressionForm_SelectionChanged(object sender, EventArgs e)
        {
            UpdateUri();
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
            Response.Redirect(Jhu.Graywulf.Web.UI.Apps.Jobs.Default.GetUrl(), false);
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

        private void UpdateUri()
        {
            var sf = FederationContext.StreamFactory;
            var ff = FederationContext.FileFormatFactory;

            foreach (IExportTablesForm form in exportForms.Values)
            {
                var uri = form.CustomizableUri;

                if (String.IsNullOrWhiteSpace(uri.ToString()))
                {
                    string tableName;
                    tableName = sourceTableForm.Table.TableName;

                    string extension;
                    DataFileBase format;
                    ff.TryCreateFileFromMimeType(fileFormatForm.FileFormat, out format);
                    extension = format?.Description.Extension ?? String.Empty;

                    DataFileCompression compression;
                    compression = compressionForm.Compression;

                    // generate new file name
                    var path = StreamFactory.CombineFileExtensions("", tableName, extension, DataFileArchival.None, compression);
                    form.GenerateDefaultUri(path);
                }
                else
                {
                    string path, filename, extension;
                    DataFileArchival archival;
                    DataFileCompression compression;
                    StreamFactory.GetFileExtensions(uri, out path, out filename, out extension, out archival, out compression);

                    if (sourceTableForm.LastTable == null ||
                        sourceTableForm.Table.UniqueKey != sourceTableForm.LastTable.UniqueKey &&
                        filename == sourceTableForm.LastTable.TableName)
                    {
                        filename = sourceTableForm.Table.TableName;
                    }

                    if (compressionForm.Compression != compressionForm.LastCompression &&
                        compression == compressionForm.LastCompression)
                    {
                        compression = compressionForm.Compression;
                    }

                    if (fileFormatForm.FileFormat != fileFormatForm.LastFileFormat)
                    {
                        DataFileBase last, current;

                        ff.TryCreateFileFromMimeType(fileFormatForm.LastFileFormat, out last);
                        ff.TryCreateFileFromMimeType(fileFormatForm.FileFormat, out current);
                        
                        if (current != null &&
                            (last == null || extension == last.Description.Extension))
                        {
                            extension = current.Description.Extension;
                        }
                    }

                    path = StreamFactory.CombineFileExtensions(path, filename, extension, archival, compression);

                    if (uri.IsAbsoluteUri)
                    {
                        var ub = new UriBuilder(uri);
                        ub.Path = path;
                        form.CustomizableUri = ub.Uri;
                    }
                    else
                    {
                        form.CustomizableUri = new Uri(path, UriKind.RelativeOrAbsolute);
                    }
                }
            }
        }

        private void ExportViaBrowser()
        {
            // TODO: add support for multi-table downloads

            var compression = compressionForm.Compression;
            var file = fileFormatForm.GetDataFile();
            var table = sourceTableForm.Table;
            var uri = StreamFactory.CombineFileExtensions("", table.ObjectName, file.Description.Extension, DataFileArchival.None, compression);
            file.Uri = new Uri(uri, UriKind.Relative);
            file.Compression = compression;

            var task = new ExportTable()
            {
                BatchName = table.ObjectName,
                Source = SourceTableQuery.Create(table),
                Destination = file,
                StreamFactoryType = FederationContext.Federation.StreamFactory,
                FileFormatFactoryType = FederationContext.Federation.FileFormatFactory,
            };

            var guid = PushSessionItem(task);
            Response.Redirect(Download.GetUrl(guid), false);
        }

        private void ScheduleExportJob()
        {
            var form = (IExportTablesForm)exportForms[exportMethod.SelectedValue];

            var uri = form.Uri;
            var credentials = form.Credentials;
            var format = fileFormatForm.GetFormat();
            var compression = compressionForm.Compression;
            var table = sourceTableForm.Table;

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

            new JobsService(FederationContext).SubmitJob(job);
        }
    }
}