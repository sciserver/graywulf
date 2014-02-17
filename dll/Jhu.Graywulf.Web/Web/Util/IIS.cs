using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Util
{
    public static class IIS
    {
        public static int MajorVersion
        {
            get
            {
                var parameters = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\W3SVC\\Parameters");
                return (int)parameters.GetValue("MajorVersion");
            }
        }
    }
}
