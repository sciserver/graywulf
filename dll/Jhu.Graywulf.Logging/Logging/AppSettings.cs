using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Logging
{
    public static class AppSettings
    {
        public static string ConnectionString
        {
            get
            {
                if (ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Logging"] != null)
                {
                    return ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Logging"].ConnectionString;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
