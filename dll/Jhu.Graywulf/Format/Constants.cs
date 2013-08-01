using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Format
{
    public static class Constants
    {
        public static readonly Map<CompressionMethod, string> CompressionExtensions = new Map<CompressionMethod,string>()
        {
            { CompressionMethod.None, "" },
            { CompressionMethod.BZip2, ".bz2" },
            { CompressionMethod.GZip, ".gz" },
            { CompressionMethod.Zip, ".zip" },
        };
    }
}
