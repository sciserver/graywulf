using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Jhu.Graywulf.Web.UI
{
    /// <summary>
    /// Summary description for Constants
    /// </summary>
    public static class AppSettings
    {
        private static string GetValue(string key)
        {
            return (string)((NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Web.UI"))[key];
        }

        public static string ExportDir
        {
            get { return GetValue("ExportDir"); }       // *** TODO: move this to the federation config?
        }
    }
}