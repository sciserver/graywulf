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

    public enum DataFileCompression
    {
        None,
        Automatic,
        BZip2,
        GZip,
        Zip
    }

    public enum DataFileArchival
    {
        None,
        Automatic,
        Tar,
        Zip
    }
}
