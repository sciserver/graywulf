using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TableSource : ITableReference, ICloneable
    {
        public abstract bool IsSubquery { get; }
        public abstract bool IsMultiTable { get; }
        public abstract string UniqueKey { get; set; }
        public abstract TableReference TableReference { get; set; }
    }
}
