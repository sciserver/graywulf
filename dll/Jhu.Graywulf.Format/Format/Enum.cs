using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format
{
    public enum DataFileMode
    {
        Unknown,
        Read,
        Write
    }

    public enum CompressionMethod
    {
        None,
        BZip2,
        GZip,
        Zip,
    }
}
