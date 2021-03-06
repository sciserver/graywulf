﻿using System;
using System.Web.Configuration;
using System.Threading.Tasks;
using System.IO;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class UploadForm : FederationUserControlBase
    {
        private Stream stream;

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

        public CopyTableBase CreateTableImporter(DataFileBase file, DestinationTable table)
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
                    Source = file,
                    Destination = table,
                    Settings = new TableCopySettings()
                    {
                        BatchName = null,       // no batch name for single files
                        StreamFactoryType = RegistryContext.Federation.StreamFactory,
                        FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
                    }
                    // TODO: generate identity?
                };

                return task;
            }
            else
            {
                // An archive is being uploaded, use archive importer
                var task = new ImportTableArchive()
                {
                    Destinations = new DestinationTable[] { table },
                    Settings = new TableCopySettings()
                    {
                        BypassExceptions = true,
                        BatchName = batchName,
                        StreamFactoryType = RegistryContext.Federation.StreamFactory,
                        FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
                        // TODO: host to pass options? Options = options,
                    }
                };

                stream = FederationContext.StreamFactory.Open(
                    importedFile.PostedFile.InputStream, 
                    DataFileMode.Read, 
                    compression, 
                    archival);

                return task;
            }
        }

        public async Task OpenTableImporter(CopyTableBase task)
        {
            switch (task)
            {
                case ImportTableArchive ita:
                    await ita.OpenAsync(stream);
                    break;
                case ImportTable it:
                    await it.OpenAsync();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}