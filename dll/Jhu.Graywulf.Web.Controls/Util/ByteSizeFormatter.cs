using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Util
{
    public class ByteSizeFormatter
    {
        private const string FS_PB = "PB";
        private const string FS_TB = "TB";
        private const string FS_GB = "GB";
        private const string FS_MB = "MB";
        private const string FS_kB = "kB";
        private const string FS_B = "B";

        private const long FL_PB = 0x4000000000000;
        private const long FL_TB = 0x10000000000;
        private const long FL_GB = 0x40000000;
        private const long FL_MB = 0x100000;
        private const long FL_kB = 0x400;

        private static readonly Regex SizeRegex = new Regex(@"([0-9]*\.[0-9]+|[0-9]+)\s*([a-zA-Z]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static long Parse(string size)
        {
            Match m = SizeRegex.Match(size);

            double res = double.Parse(m.Groups[1].Value);
            string unit = m.Groups[2].Value;

            if (string.Compare(unit, FS_B, true) == 0 || unit == string.Empty)
            {
                // no multiplier for bytes
            }
            else if (string.Compare(unit, FS_kB, true) == 0)
            {
                res *= FL_kB;
            }
            else if (string.Compare(unit, FS_MB, true) == 0)
            {
                res *= FL_MB;
            }
            else if (string.Compare(unit, FS_GB, true) == 0)
            {
                res *= FL_GB;
            }
            else if (string.Compare(unit, FS_TB, true) == 0)
            {
                res *= FL_TB;
            }
            else if (string.Compare(unit, FS_PB, true) == 0)
            {
                res *= FL_PB;
            }
            else
            {
                throw new ArgumentException();
            }

            return (long)res;
        }

        public static string Format(long size)
        {
            double res;
            string unit;

            if (size < FL_kB)
            {
                res = size;
                unit = FS_B;
            }
            else if (size >= FL_kB && size < FL_MB)
            {
                res = (double)size / FL_kB;
                unit = FS_kB;
            }
            else if (size >= FL_MB && size < FL_GB)
            {
                res = (double)size / FL_MB;
                unit = FS_MB;
            }
            else if (size >= FL_GB && size < FL_TB)
            {
                res = (double)size / FL_GB;
                unit = FS_GB;
            }
            else if (size >= FL_TB && size < FL_PB)
            {
                res = (double)size / FL_TB;
                unit = FS_TB;
            }
            else
            {
                res = (double)size / FL_PB;
                unit = FS_PB;
            }

            return res.ToString("0.###") + " " + unit;
        }
    }
}
