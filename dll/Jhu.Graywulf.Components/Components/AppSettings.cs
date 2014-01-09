using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Components
{
    public static class AppSettings
    {
        private static object GetValue(string key)
        {
            return ((NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Components"))[key];
        }

        public static string AssemblyPath
        {
            get { return (string)GetValue("AssemblyPath"); }
        }
    }
}
