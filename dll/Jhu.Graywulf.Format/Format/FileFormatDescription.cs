using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format
{
    public class FileFormatDescription
    {
        public Type Type { get; set; }
        public string DisplayName { get; set; }
        public string DefaultExtension { get; set; }
        public bool CanCompress { get; set; }
    }
}
