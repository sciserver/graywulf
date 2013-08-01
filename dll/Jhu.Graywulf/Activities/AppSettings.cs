using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Activities
{
    public static class AppSettings
    {
        private static object GetValue(string key)
        {
            return ((NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Activities"))[key];
        }

        public static string WorkflowAssemblyPath
        {
            get { return (string)GetValue("WorkflowAssemblyPath"); }
        }
    }
}
