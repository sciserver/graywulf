using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class FunctionReferenceEqualityComparer : DatabaseObjectReferenceEqualityComparer<FunctionReference>
    {
        private static volatile FunctionReferenceEqualityComparer defaultComparer;

        public static new FunctionReferenceEqualityComparer Default
        {
            get
            {
                if (defaultComparer == null)
                {
                    defaultComparer = new FunctionReferenceEqualityComparer();
                }

                return defaultComparer;
            }
        }

        public override bool Equals(FunctionReference x, FunctionReference y)
        {
            bool res =
                SchemaManager.Comparer.Compare(x.SystemFunctionName, y.SystemFunctionName) == 0 &&
                base.Equals(x, y);

            return res;
        }

        public override int GetHashCode(FunctionReference obj)
        {
            var res =
                (obj.SystemFunctionName == null ? 0 : obj.SystemFunctionName.GetHashCode()) +
                base.GetHashCode(obj);

            return res;
        }
    }
}
