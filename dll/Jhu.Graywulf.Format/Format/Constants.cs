using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Format
{
    public static class Constants
    {
        public static readonly Map<string, CompressionMethod> CompressionExtensions = new Map<string, CompressionMethod>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "", CompressionMethod.None  },
            { ".bz2", CompressionMethod.BZip2 },
            { ".gz", CompressionMethod.GZip },
            { ".zip", CompressionMethod.Zip },
        };
    }
}
