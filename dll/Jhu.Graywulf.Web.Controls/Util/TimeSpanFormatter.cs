using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class TimeSpanFormatter
    {
        public static string Format(TimeSpan? value)
        {
            if (value.HasValue)
            {
                return Format(value.Value);
            }
            else
            {
                return "";
            }
        }

        public static string Format(TimeSpan value)
        {
            if (value == TimeSpan.MinValue)
            {
                return "";
            }
            else
            {
                return value.ToString();
            }
        }

        public static string FancyFormat(TimeSpan? value)
        {
            if (!value.HasValue)
            {
                return "";
            }
            else
            {
                return FancyFormat(value.Value);
            }
        }

        public static string FancyFormat(TimeSpan value)
        {
            value = value.Duration();

            if (value == TimeSpan.MinValue)
            {
                return "";
            }
            else if (value.TotalSeconds < 60)
            {
                return String.Format("{0:s\\.f} sec", value);
            }
            else if (value.TotalMinutes < 60)
            {
                return String.Format("{0:m\\:ss} min", value);
            }
            else if (value.TotalHours < 24)
            {
                return String.Format("{0:h\\:mm} hour", value);
            }
            else
            {
                return String.Format("more than {0} day{1}", (int)Math.Floor(value.TotalDays), value.TotalDays >= 2 ? "s" : "");
            }
        }
    }
}
