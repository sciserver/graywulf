using System;
using System.Collections.Generic;
using System.Linq;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class Predicate
    {
        public bool IsSpecificToTable(TableReference table)
        {
            foreach (var tr in NodeExtensions.EnumerateTableReferences(this))
            {
                if (tr != null && !tr.Compare(table))
                {
                    return false;
                }
            }

            return true;
        }

        public LogicalExpressions.Expression GetExpressionTree()
        {
            return new LogicalExpressions.Predicate(this);
        }
    }
}
