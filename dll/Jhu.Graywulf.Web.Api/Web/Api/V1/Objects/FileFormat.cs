using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a file format.")]
    public class FileFormat
    {
        [DataMember(Name = "mimeType", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("MIME type of file. Overrides type infered from extension.")]
        public string MimeType { get; set; }

        [DataMember(Name = "generateIdentityColumn", EmitDefaultValue = false)]
        [DefaultValue(false)]
        [Description("If true, automatically generates identity column on new tables.")]
        public bool GenerateIdentityColumn { get; set; }

        public DataFileBase GetDataFile(FederationContext context, Uri uri)
        {
            DataFileBase file = null;
            var archival = context.StreamFactory.GetArchivalMethod(uri);

            // If it is not an archive, URI must be set directly on the file
            // TODO: uri might also be set directly when exporting into an archive
            if (archival == DataFileArchival.None)
            {
                if (MimeType != null)
                {
                    // Create a specific file type
                    file = context.FileFormatFactory.CreateFileFromMimeType(MimeType);
                }
                else
                {
                    //
                    file = context.FileFormatFactory.CreateFile(uri);
                }

                file.Uri = uri;
            }
            else
            {
                if (MimeType != null)
                {
                    // Create a specific file type
                    file = context.FileFormatFactory.CreateFileFromMimeType(MimeType);
                }

                // TODO: it might be a single compressed file... test
            }

            if (file != null)
            {
                file.GenerateIdentityColumn = GenerateIdentityColumn;
            }

            // TODO: add compression option for export

            return file;
        }
    }
}
