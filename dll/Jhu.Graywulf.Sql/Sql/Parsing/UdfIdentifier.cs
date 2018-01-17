using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdfIdentifier
    {
        public static UdfIdentifier Create()
        {
            var udfi = new UdfIdentifier();
            return udfi;
        }
    }
}
