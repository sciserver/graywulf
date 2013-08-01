using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.RemoteService
{
    public static class AppSettings
    {
        private static object GetValue(string key)
        {
            return ((NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/RemoteService"))[key];
        }

        public static int TcpPort
        {
            get { return int.Parse((string)GetValue("TcpPort")); }
        }

        public static string UserGroup
        {
            get { return (string)GetValue("UserGroup"); }
        }
    }
}
