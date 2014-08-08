using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class EnumFormatter
    {
        public static string ToXmlString<T>(T value)
            where T : struct
        {
            return value.ToString().ToLowerInvariant();
        }

        public static T FromXmlString<T>(string text)
            where T : struct
        {
            T value;
            Enum.TryParse<T>(text, true, out value);
            return value;
        }
    }
}
