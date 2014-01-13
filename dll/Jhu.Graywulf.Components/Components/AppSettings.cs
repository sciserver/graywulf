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
            var collection = (NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Components");

            if (collection != null)
            {
                return collection[key];
            }
            else
            {
                return null;
            }
        }

        public static string AssemblyPath
        {
            get
            {
                return (string)GetValue("AssemblyPath") ?? "";
            }
        }
    }
}
