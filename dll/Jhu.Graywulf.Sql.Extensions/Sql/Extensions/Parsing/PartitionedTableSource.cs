using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Extensions.Parsing
{
    public partial class PartitionedTableSource
    {
        private Sql.Parsing.ColumnIdentifier partitioningColumn;
        private Expression partitioningKeyExpression;
        private Schema.DataType partitioningKeyDataType;

        public bool IsPartitioned
        {
            get { return FindDescendant<TablePartitionClause>() != null; }
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

        private void InterpretPartitioningKey()
        {
            var tpc = this.FindDescendant<TablePartitionClause>();

            if (tpc != null)
            {
                var ci = tpc.FindDescendant<Sql.Parsing.ColumnIdentifier>();
                partitioningColumn = ci;
                partitioningKeyExpression = Expression.Create(ci);
                partitioningKeyDataType = ci.ColumnReference.DataTypeReference?.DataType;
            }
        }
    }
}
