using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Format
{
    public static class AppSettings
    {
        private static string GetValue(string key)
        {
            return (string)((NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Format"))[key];
        }

        public static string FileFormatFactory
        {
            get { return GetValue("FileFormatFactory"); }
        }

        public static string StreamFactory
        {
            get { return GetValue("StreamFactory"); }
        }
    }
}
