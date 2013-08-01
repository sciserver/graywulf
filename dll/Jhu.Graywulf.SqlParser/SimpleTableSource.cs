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
            set { TableOrViewName.TableReference = value ; }
        }

        public ColumnReference PartitioningColumnReference
        {
            get
            {
                TablePartitionClause tpc = this.FindDescendant<TablePartitionClause>();
                if (tpc != null)
                {
                    ColumnIdentifier ci = tpc.FindDescendant<ColumnIdentifier>();
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
    }
}
