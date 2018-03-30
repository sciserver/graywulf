using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public abstract class DatabaseObjectReferenceEqualityComparer<T> : EqualityComparer<T>
        where T : DatabaseObjectReference
    {
        public override bool Equals(T x, T y)
        {
            bool res =
                SchemaManager.Comparer.Compare(x.DatasetName, y.DatasetName) == 0 &&
                SchemaManager.Comparer.Compare(x.DatabaseName, y.DatabaseName) == 0 &&
                SchemaManager.Comparer.Compare(x.SchemaName, y.SchemaName) == 0 &&
                SchemaManager.Comparer.Compare(x.DatabaseObjectName, y.DatabaseObjectName) == 0;

            return res;
        }

        public override int GetHashCode(T obj)
        {
            var res =
                (obj.DatasetName == null ? 0 : obj.DatasetName.GetHashCode()) +
                (obj.DatabaseName == null ? 0 : obj.DatabaseName.GetHashCode()) +
                (obj.SchemaName == null ? 0 : obj.SchemaName.GetHashCode()) +
                (obj.DatabaseObjectName == null ? 0 : obj.DatabaseObjectName.GetHashCode());

            return res;
        }
    }
}
