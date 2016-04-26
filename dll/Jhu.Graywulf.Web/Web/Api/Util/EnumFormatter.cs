using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Api.Util
{
    public static class EnumFormatter
    {
        public static string ToXmlString<T>(T value)
            where T : struct
        {
            return value.ToString().ToLowerInvariant();
        }

        public static string ToNullableXmlString<T>(Nullable<T> value)
            where T : struct
        {
            if (value.HasValue)
            {
                return value.ToString().ToLowerInvariant();
            }
            else
            {
                return null;
            }
        }

        public static T FromXmlString<T>(string text)
            where T : struct
        {
            T value;
            Enum.TryParse<T>(text, true, out value);
            return value;
        }

        public static Nullable<T> FromNullableXmlString<T>(string text)
            where T : struct
        {
            if (text == null)
            {
                return null;
            }
            else
            {
                T value;
                Enum.TryParse<T>(text, true, out value);
                return value;
            }
        }
    }
}
