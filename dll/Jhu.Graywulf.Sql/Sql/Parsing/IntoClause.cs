using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class IntoClause
    {
        public TableOrViewName TableName
        {
            get { return FindDescendantRecursive<TableOrViewName>(); }
        }

        public UserVariable Variable
        {
            get { return FindDescendantRecursive<UserVariable>(); }
        }
    }
}
