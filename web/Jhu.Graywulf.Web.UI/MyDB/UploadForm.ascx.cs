using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class UploadForm : CustomUserControlBase
    {
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

        public CopyTableBase GetTableImporter(DataFileBase file, DestinationTable table)
        {
            var uri = this.Uri;

            // Check if uploaded file is an archive
            var archival = FederationContext.StreamFactory.GetArchivalMethod(uri);
            var compression = FederationContext.StreamFactory.GetCompressionMethod(uri);
            var batchName = Util.UriConverter.ToFileNameWithoutExtension(uri);

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
                    BatchName = batchName,
                    Source = file,
                    Destination = table,
                    StreamFactoryType = RegistryContext.Federation.StreamFactory,
                    FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
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
                };

                task.Open(FederationContext.StreamFactory.Open(importedFile.PostedFile.InputStream, DataFileMode.Read, compression, archival));

                return task;
            }
        }
    }
}