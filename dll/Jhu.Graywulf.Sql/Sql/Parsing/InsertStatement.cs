using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class InsertStatement : ISourceTableProvider, ITargetTableProvider
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

        public ColumnIdentifierList ColumnList
        {
            get
            {
                return FindDescendant<InsertColumnList>()?.FindDescendant<ColumnIdentifierList>();
            }
        }

        public ValuesClause ValuesClause
        {
            get { return FindDescendant<ValuesClause>(); }
        }

        public bool IsDefaultValues
        {
            get
            {
                // TODO: handle special syntax of DEFAULT VALUES
                throw new NotImplementedException();
            }
        }

        public QueryExpression QueryExpression
        {
            get { return FindDescendant<QueryExpression>(); }
        }

        public OrderByClause OrderByClause
        {
            get { return FindDescendant<OrderByClause>(); }
        }

        public OptionClause OptionClause
        {
            get { return FindDescendant<OptionClause>(); }
        }    }
}
