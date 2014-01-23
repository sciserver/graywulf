using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class EmailFormatter
    {
        public static string ToUsername(string email)
        {
            var idx = email.IndexOf('@');
            return email.Substring(0, idx);
        }
    }
}
