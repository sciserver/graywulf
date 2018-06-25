using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class ColumnReferenceEqualityComparer : EqualityComparer<ColumnReference>
    {
        private static volatile ColumnReferenceEqualityComparer defaultComparer;

        public static new ColumnReferenceEqualityComparer Default
        {
            get
            {
                if (defaultComparer == null)
                {
                    defaultComparer = new ColumnReferenceEqualityComparer();
                }

                return defaultComparer;
            }
        }
        
        public override bool Equals(ColumnReference x, ColumnReference y)
        {
            bool res =
                x.IsStar == y.IsStar &&
                x.IsComplexExpression == y.IsComplexExpression &&
                SchemaManager.Comparer.Compare(x.ColumnName, y.ColumnName) == 0 &&
                SchemaManager.Comparer.Compare(x.ColumnAlias, y.ColumnAlias) == 0 &&
                TableReferenceEqualityComparer.Default.Equals(x.ParentTableReference, y.ParentTableReference);
            return res;
        }

        public override int GetHashCode(ColumnReference obj)
        {
            var res =
                obj.IsStar.GetHashCode() +
                obj.IsComplexExpression.GetHashCode() +
                (obj.ColumnName == null ? 0 : obj.ColumnName.GetHashCode()) +
                (obj.ColumnAlias == null ? 0 : obj.ColumnAlias.GetHashCode()) +
                (obj.ParentTableReference == null ? 0 : TableReferenceEqualityComparer.Default.GetHashCode(obj.ParentTableReference));

            return res;
        }
    }
}
