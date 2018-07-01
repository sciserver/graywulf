using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class NumericConstant
    {
        public static NumericConstant Create(int value)
        {
            return NumericConstant.Create(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public static NumericConstant Create(double value)
        {
            return NumericConstant.Create(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
