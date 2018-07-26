using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UpdateStatement : ISourceTableProvider, ITargetTableProvider
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

        public UpdateSetList UpdateSetList
        {
            get { return FindDescendant<UpdateSetList>(); }
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
