using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Util
{
    public static class ConfigurationReader
    {
        public static string GetValue(string group, string key)
        {
            var section = (NameValueCollection)ConfigurationManager.GetSection(group);
            if (section != null)
            {
                return (string)section[key];
            }
            else
            {
                return null;
            }
        }
    }
}
