using System;
using System.Collections.Generic;
using System.Linq;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SimpleTableSource : ITableSource
    {
        private string uniqueKey;
        private ColumnIdentifier partitioningColumn;
        private Expression partitioningKeyExpression;
        private Schema.DataType partitioningKeyDataType;

        public TableOrViewIdentifier TableOrViewIdentifier
        {
            get { return FindDescendant<TableOrViewIdentifier>(); }
        }

        public TableAlias Alias
        {
            get { return FindDescendant<TableAlias>(); }
        }

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return TableOrViewIdentifier.TableReference; }
        }

        public TableReference TableReference
        {
            get { return TableOrViewIdentifier.TableReference; }
            set { TableOrViewIdentifier.TableReference = value; }
        }

        public bool IsSubquery
        {
            get { return false; }
        }

        public bool IsMultiTable
        {
            get { return false; }
        }

        public string UniqueKey
        { 
            get { return uniqueKey; }
            set { uniqueKey = value; }
        }

        public Expression PartitioningKeyExpression
        {
            get
            {
                if (partitioningKeyExpression == null)
                {
                    InterpretPartitioningKey();
                }

                return partitioningKeyExpression;
            }
            set
            {
                partitioningKeyExpression = value;
            }
        }

        public Schema.DataType PartitioningKeyDataType
        {
            get
            {
                if (partitioningKeyDataType == null)
                {
                    InterpretPartitioningKey();
                }

                return partitioningKeyDataType;
            }
            set
            {
                partitioningKeyDataType = value;
            }
        }

        public bool IsPartitioned
        {
            get { return FindDescendant<TablePartitionClause>() != null; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.partitioningColumn = null;
            this.partitioningKeyExpression = null;
            this.partitioningKeyDataType = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);


            // Lazy-loaded, keep them null
            this.partitioningColumn = null;
            this.partitioningKeyExpression = null;
            this.partitioningKeyDataType = null;
        }

        public static SimpleTableSource Create(TableReference tr)
        {
            var ts = new SimpleTableSource();
            var name = TableOrViewIdentifier.Create(tr);

            ts.Stack.AddLast(name);

            if (!String.IsNullOrWhiteSpace(tr.Alias))
            {
                ts.Stack.AddLast(Whitespace.Create());
                ts.Stack.AddLast(Keyword.Create("AS"));
                ts.Stack.AddLast(Whitespace.Create());
                ts.Stack.AddLast(TableAlias.Create(tr.Alias));
            }

            return ts;
        }

        public override void Interpret()
        {
            base.Interpret();

            TableReference = TableReference.Interpret(this);
        }

        private void InterpretPartitioningKey()
        {
            var tpc = this.FindDescendant<TablePartitionClause>();

            if (tpc != null)
            {
                var ci = tpc.FindDescendant<ColumnIdentifier>();
                partitioningColumn = ci;
                partitioningKeyExpression = Expression.Create(ci);
                partitioningKeyDataType = ci.ColumnReference.DataTypeReference.DataType;
            }
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
