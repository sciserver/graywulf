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
            // TODO: assume UTC but display local time?

            if (value == DateTime.MinValue)
            {
                return "";
            }
            else
            {
                DateTime now;
                if (value.Kind == DateTimeKind.Local)
                {
                    now = DateTime.Now;
                }
                else
                {
                    now = DateTime.UtcNow;
                }

                TimeSpan elapsed = now - value;
                
                if (elapsed.TotalSeconds > 0)
                {
                    // Past
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
                        return String.Format("{0}:{1:D2} minutes ago", elapsed.Minutes, elapsed.Seconds);
                    }
                    else if (value.Date == now.Date)
                    {
                        return String.Format("today, {0:HH:mm:ss}", value);
                    }
                    else if (value.Date == now.Date.AddDays(-1))
                    {
                        return String.Format("yesterday, {0:HH:mm:ss}", value);
                    }
                    else
                    {
                        return String.Format("{0:yy-MM-dd HH:mm:ss}", value);
                    }
                }
                else
                {
                    elapsed = elapsed.Duration();

                    // Future
                    if (elapsed.TotalSeconds < 5)
                    {
                        return "now";
                    }
                    else if (elapsed.TotalMinutes < 1)
                    {
                        return String.Format("{0:D2} seconds from now", elapsed.Seconds);
                    }
                    else if (elapsed.TotalHours < 1)
                    {
                        return String.Format("{0}:{1:D2} minutes from now", elapsed.Minutes, elapsed.Seconds);
                    }
                    else if (value.Date == now.Date)
                    {
                        return String.Format("today, {0:HH:mm:ss}", value);
                    }
                    else if (value.Date == now.Date.AddDays(1))
                    {
                        return String.Format("tomorrow, {0:HH:mm:ss}", value);
                    }
                    else
                    {
                        return String.Format("{0:yy-MM-dd HH:mm:ss}", value);
                    }
                }
            }
        }
    }
}
