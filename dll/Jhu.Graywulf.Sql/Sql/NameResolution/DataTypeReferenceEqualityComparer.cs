using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class DataTypeReferenceEqualityComparer : DatabaseObjectReferenceEqualityComparer<DataTypeReference>
    {
        private static volatile DataTypeReferenceEqualityComparer defaultComparer;

        public static new DataTypeReferenceEqualityComparer Default
        {
            get
            {
                if (defaultComparer == null)
                {
                    defaultComparer = new DataTypeReferenceEqualityComparer();
                }

                return defaultComparer;
            }
        }
    }
}
