using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format
{
    public class FileFormatMapping
    {
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public Type Type { get; set; }
    }
}
