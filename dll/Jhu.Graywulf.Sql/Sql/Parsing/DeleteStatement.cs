using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DeleteStatement : ISourceTableProvider, ITargetTableProvider
    {
        public CommonTableExpression CommonTableExpression
        {
            get { return FindDescendant<CommonTableExpression>(); }
        }

        public TargetTableSpecification TargetTable
        {
            get { return FindDescendant<TargetTableSpecification>(); }
        }

        public TableReference TargetTableReference
        {
            get { return TargetTable.TableReference; }
        }

        public FromClause FromClause
        {
            get { return FindDescendant<FromClause>(); }
        }

        public WhereClause WhereClause
        {
            get { return FindDescendant<WhereClause>(); }
        }
    }
}
