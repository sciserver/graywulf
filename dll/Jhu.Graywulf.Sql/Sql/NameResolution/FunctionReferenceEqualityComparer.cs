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
            bool res = base.Equals(x, y);

            return res;
        }

        public override int GetHashCode(FunctionReference obj)
        {
            var res = base.GetHashCode(obj);

            return res;
        }
    }
}
