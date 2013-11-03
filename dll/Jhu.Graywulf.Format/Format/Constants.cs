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

        public static readonly Map<string, CompressionMethod> CompressionExtensions = new Map<string, CompressionMethod>(StringComparer.InvariantCultureIgnoreCase)
        {
            { String.Empty, CompressionMethod.None  },
            { FileExtensionBz2, CompressionMethod.BZip2 },
            { FileExtensionGz, CompressionMethod.GZip },
            { FileExtensionZip, CompressionMethod.Zip },
        };
    }
}
