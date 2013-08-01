using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class DateFormatter
    {
        public static string Format(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return "";
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
