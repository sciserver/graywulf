using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.IO
{
    public static class Constants
    {
        public const int DefaultBulkInsertBatchSize = 0;
        public const int DefaultBulkInsertTimeout = 1000;   // in sec

        public const string FileExtensionBz2 = ".bz2";
        public const string FileExtensionGz = ".gz";
        public const string FileExtensionZip = ".zip";
        public const string FileExtensionTar = ".tar";

        // Mime types taken from wikipedia
        public const string MimeTypeBz2 = "application/x-bzip2";
        public const string MimeTypeGz = "application/gzip";
        public const string MimeTypeZip = "application/zip";
        public const string MimeTypeTar = "application/x-tar";

        public const string UriSchemeHttp = "http";
        public const string UriSchemeHttps = "https";
        public const string UriSchemeFtp = "ftp";

        public const string ResultsetNameToken = "[$ResultsetName]";

        public const string UrlPattern = @"(https?|ftp)://(-\.)?([^\s/?\.#-]+\.?)+(/[^\s]*)?";
        public const string UrlPathPattern = @"([^\s/?\.#-]+\.?)+(/[^\s]*)?";


        public static readonly Map<string, DataFileCompression> CompressionExtensions = new Map<string, DataFileCompression>(StringComparer.InvariantCultureIgnoreCase)
        {
            { String.Empty, DataFileCompression.None  },
            { FileExtensionBz2, DataFileCompression.BZip2 },
            { FileExtensionGz, DataFileCompression.GZip },
            { FileExtensionZip, DataFileCompression.Zip },
        };

        public static readonly Map<string, DataFileCompression> CompressionMimeTypes = new Map<string, DataFileCompression>(StringComparer.InvariantCultureIgnoreCase)
        {
            { String.Empty, DataFileCompression.None  },
            { MimeTypeBz2, DataFileCompression.BZip2 },
            { MimeTypeGz, DataFileCompression.GZip },
            { MimeTypeZip, DataFileCompression.Zip },
        };

        public static readonly Map<string, DataFileArchival> ArchivalExtensions = new Map<string, DataFileArchival>(StringComparer.InvariantCultureIgnoreCase)
        {
            { String.Empty, DataFileArchival.None },
            { FileExtensionTar, DataFileArchival.Tar },
            { FileExtensionZip, DataFileArchival.Zip },
        };
    }
}
