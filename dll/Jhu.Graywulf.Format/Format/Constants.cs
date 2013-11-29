using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Format
{
    public static class Constants
    {
        public const string FileExtensionCsv = ".csv";
        public const string FileExtensionBz2 = ".bz2";
        public const string FileExtensionGz = ".gz";
        public const string FileExtensionZip = ".zip";
        public const string FileExtensionTar = ".tar";

        public const string UriSchemeHttp = "http";
        public const string UriSchemeHttps = "https";
        public const string UriSchemeFtp = "ftp";

        public static readonly Map<string, DataFileCompression> CompressionExtensions = new Map<string, DataFileCompression>(StringComparer.InvariantCultureIgnoreCase)
        {
            { String.Empty, DataFileCompression.None  },
            { FileExtensionBz2, DataFileCompression.BZip2 },
            { FileExtensionGz, DataFileCompression.GZip },
            { FileExtensionZip, DataFileCompression.Zip },
        };
    }
}
