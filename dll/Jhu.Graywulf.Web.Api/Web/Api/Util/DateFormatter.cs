using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Api.Util
{
    public static class DateFormatter
    {
        public static string ToXmlString(DateTime? value)
        {
            if (value.HasValue)
            {
                return value.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return null;
            }
        }

        public static DateTime? FromXmlString(string text)
        {
            if (text != null)
            {
                return DateTime.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return null;
            }
        }
    }
}
