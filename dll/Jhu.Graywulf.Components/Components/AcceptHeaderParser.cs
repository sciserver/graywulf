using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Components
{
    public class AcceptHeaderParser
    {
        private enum Parameter
        {
            Unknown,
            Level,
            Quality,
        }

        private static readonly Regex AcceptHeaderRegex = new Regex(@"(([a-z-\.\+\*]+)(?:/([a-z-\.\+\*]+))?)(?:\s*;\s*(q|level)\s*=\s*([0-9\.]+))?(?:\s*;\s*(q|level)\s*=\s*([0-9\.]+))?");

        public static AcceptMimeType[] Parse(string value)
        {
            var parts = value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var res = new AcceptMimeType[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                res[i] = GetMimeType(parts[i]);
                res[i].Index = i;
            }

            // Sort results by quality in a descending order
            Array.Sort(res);

            return res;
        }

        private static AcceptMimeType GetMimeType(string haederPart)
        {
            var m = AcceptHeaderRegex.Match(haederPart);

            var res = new AcceptMimeType();
            res.MimeType = m.Groups[1].Value;

            int i = 4;
            while ( i < m.Groups.Count)
            {
                Parameter parameter;
                double value;

                if (m.Groups[i].Success && GetParameter(m, i, out parameter, out value))
                {
                    switch (parameter)
                    {
                        case Parameter.Level:
                            res.Level = (int)value;
                            break;
                        case Parameter.Quality:
                            res.Quality = value;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                i += 2;
            }

            return res;
        }

        private static bool GetParameter(Match m, int i, out Parameter parameter, out double value)
        {
            if (m.Groups[i].Success)
            {
                switch (m.Groups[i].Value.ToLowerInvariant())
                {
                    case "level":
                        parameter = Parameter.Level;
                        break;
                    case "q":
                        parameter = Parameter.Quality;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                value = Double.Parse(m.Groups[i + 1].Value, System.Globalization.CultureInfo.InvariantCulture);

                return true;
            }
            else
            {
                parameter =  Parameter.Unknown;
                value = 0;
                return false;
            }
        }
    }
}
