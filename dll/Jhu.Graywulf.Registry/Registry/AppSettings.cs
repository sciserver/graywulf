using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Registry
{
    public static class AppSettings
    {
        private static string GetValue(string key)
        {
            return (string)((NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Registry"))[key];
        }

        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Registry"].ConnectionString; }
        }
    }
}
