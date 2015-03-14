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
    [DataContract(Name = "fileFormat")]
    [Description("Represents a file format.")]
    public class FileFormat
    {
        [DataMember(Name = "mimeType", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("MIME type of file. Overrides type infered from extension.")]
        public string MimeType { get; set; }

        // TODO: additional file options

        public DataFileBase GetDataFile(FederationContext context, Uri uri)
        {
            DataFileBase file = null;

            if (MimeType != null)
            {
                // Create a specific file type
                file = context.FileFormatFactory.CreateFileFromMimeType(MimeType);
            }
            else
            {
                //
                file = context.FileFormatFactory.CreateFileFromExtension(Jhu.Graywulf.Util.UriConverter.ToExtension(uri));
            }

            // TODO: add compression option for export

            return file;
        }
    }
}
