using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class VariableReferenceEqualityComparer : EqualityComparer<VariableReference>
    {
        private static volatile VariableReferenceEqualityComparer defaultComparer;

        public static new VariableReferenceEqualityComparer Default
        {
            get
            {
                if (defaultComparer == null)
                {
                    defaultComparer = new VariableReferenceEqualityComparer();
                }

                return defaultComparer;
            }
        }

        public override bool Equals(VariableReference x, VariableReference y)
        {
            bool res =
                SchemaManager.Comparer.Compare(x.VariableName, y.VariableName) == 0;

            return res;
        }

        public override int GetHashCode(VariableReference obj)
        {
            var res =
                (obj.VariableName == null ? 0 : obj.VariableName.GetHashCode());

            return res;
        }
    }
}
