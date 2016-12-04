using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class DateFormatter
    {
        public static string Format(DateTime? value)
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

        public static string FancyFormat(DateTime? value)
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

        public static string FancyFormat(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return "";
            }
            else
            {
                var elapsed = DateTime.Now - value;

                if (elapsed.TotalSeconds < 5)
                {
                    return "now";
                }
                else if (elapsed.TotalMinutes < 1)
                {
                    return String.Format("{0:D2} seconds ago", elapsed.Seconds);
                }
                else if (elapsed.TotalHours < 1)
                {
                    return String.Format("{0}:{1:D2} ago", elapsed.Minutes, elapsed.Seconds);
                }
                else if (elapsed.TotalDays < 1)
                {
                    return String.Format("today, {0:HH:mm:ss}", value);
                }
                else if (elapsed.TotalDays < 2)
                {
                    return String.Format("yesterday, {0:HH:mm:ss}", value);
                }
                else
                {
                    return String.Format("{0:yy-MM-dd HH:mm:ss}", value);
                }
            }
        }
    }
}
