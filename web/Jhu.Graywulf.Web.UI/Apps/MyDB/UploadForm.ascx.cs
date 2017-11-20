using System;
using System.Web.Configuration;
using System.Xml;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class UploadForm : FederationUserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            var section = (HttpRuntimeSection)configuration.GetSection("system.web/httpRuntime");
            maxRequestLength.Text = (section.MaxRequestLength / 1024).ToString();
        }

        public Uri Uri
        {
            get
            {

                // Now this is tricky here because we don't know anything about the
                // file path format the browser sends us. Lets's assume it has a '/', '\' or ':'
                // in the path before the file name, or if not, the whole string is
                // a file name.

                var filename = importedFile.PostedFile.FileName;
                int i;

                i = filename.LastIndexOf('/');
                i = Math.Max(i, filename.LastIndexOf('\\'));
                i = Math.Max(i, filename.LastIndexOf(':'));

                if (i >= 0)
                {
                    filename = filename.Substring(i + 1);
                }

                // TODO: test this method with all browses and various types
                // containing fancy characters

                return new Uri(filename, UriKind.Relative);
            }
        }

        public CopyTableBase GetTableImporter(DataFileBase file, DestinationTable table, ImportTableOptions options)
        {
            var uri = this.Uri;

            // Check if uploaded file is an archive
            var archival = FederationContext.StreamFactory.GetArchivalMethod(uri);
            var compression = FederationContext.StreamFactory.GetCompressionMethod(uri);
            var batchName = Util.UriConverter.GetFileNameWithoutExtension(uri).Replace('.', '_');

            if (archival == DataFileArchival.None)
            {
                // A single file is being uploaded

                file.BaseStream = FederationContext.StreamFactory.Open(
                    importedFile.PostedFile.InputStream,
                    DataFileMode.Read,
                    file.Compression,
                    DataFileArchival.None);

                // Use simple importer
                var task = new ImportTable()
                {
                    BatchName = null,       // no batch name for single files
                    Source = file,
                    Destination = table,
                    StreamFactoryType = RegistryContext.Federation.StreamFactory,
                    FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
                    Options = options,
                };

                task.Open();

                return task;
            }
            else
            {
                // An archive is being uploaded, use archive importer
                var task = new ImportTableArchive()
                {
                    BypassExceptions = true,
                    BatchName = batchName,
                    Destination = table,
                    StreamFactoryType = RegistryContext.Federation.StreamFactory,
                    FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
                    Options = options,
                };

                // TODO: make async
                Util.TaskHelper.Wait(task.OpenAsync(FederationContext.StreamFactory.Open(importedFile.PostedFile.InputStream, DataFileMode.Read, compression, archival)));

                return task;
            }
        }
    }
}