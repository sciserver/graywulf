using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.NameResolution
{
    /// <summary>
    /// Compares two table references solely on the basis of name parts
    /// </summary>
    public class TableReferenceEqualityComparer : DatabaseObjectReferenceEqualityComparer<TableReference>
    {
        private static volatile TableReferenceEqualityComparer defaultComparer;

        public static new TableReferenceEqualityComparer Default
        {
            get
            {
                if (defaultComparer == null)
                {
                    defaultComparer = new TableReferenceEqualityComparer();
                }

                return defaultComparer;
            }
        }

        public override bool Equals(TableReference x, TableReference y)
        {
            bool res = 
                SchemaManager.Comparer.Compare(x.Alias, y.Alias) == 0 &&
                base.Equals(x, y);
            return res;
        }

        public override int GetHashCode(TableReference obj)
        {
            var res =
                (obj.Alias == null ? 0 : obj.Alias.GetHashCode()) +
                base.GetHashCode(obj);
            return res;
        }
    }
}
