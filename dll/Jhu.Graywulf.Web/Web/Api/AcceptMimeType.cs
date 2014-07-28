using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Api
{
    public class AcceptMimeType
    {
        public string MimeType { get; set; }
        public int? Level { get; set; }
        public double? Quality { get; set; }
    }
}
