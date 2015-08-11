using System;
using System.Collections.Generic;
using System.Linq;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class SimpleTableSource : ITableSource
    {
        public TableOrViewName TableOrViewName
        {
            get { return FindDescendant<TableOrViewName>(); }
        }

        public TableReference TableReference
        {
            get { return TableOrViewName.TableReference; }
            set { TableOrViewName.TableReference = value; }
        }

        public bool IsSubquery
        {
            get { return false; }
        }

        public bool IsMultiTable
        {
            get { return false; }
        }

        public ColumnIdentifier PartitioningColumn
        {
            get
            {
                var tpc = this.FindDescendant<TablePartitionClause>();

                if (tpc != null)
                {
                    var ci = tpc.FindDescendant<ColumnIdentifier>();
                    return ci;
                }
                else
                {
                    return null;
                }
            }
        }

        public ColumnReference PartitioningColumnReference
        {
            get
            {
                var ci = PartitioningColumn;

                if (ci != null)
                {
                    return ci.ColumnReference;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsPartitioned
        {
            get { return FindDescendant<TablePartitionClause>() != null; }
        }

        public override Node Interpret()
        {
            var node = (SimpleTableSource)base.Interpret();

            // Look up table alias
            node.TableReference.InterpretTableSource(this);

            return node;
        }

        public IEnumerable<ITableSource> EnumerateSubqueryTableSources(bool recursive)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITableSource> EnumerateMultiTableSources()
        {
            throw new NotImplementedException();
        }
    }
}
