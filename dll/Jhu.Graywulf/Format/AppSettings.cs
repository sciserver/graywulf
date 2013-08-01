using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Format
{
    public static class AppSettings
    {
        public static NameValueCollection GetFileFormats()
        {
            return (NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Format");
        }
    }
}
