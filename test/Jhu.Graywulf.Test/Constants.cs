using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Test
{
    public static class Constants
    {
        public static readonly string Localhost = Environment.MachineName;

        // Although these are defined in the registry, need to be repeated
        // to avoid the need to reference the registry dll from all projects
        public static readonly string TestDatasetName = "TEST";
        public static readonly string CodeDatasetName = "CODE";

        public static readonly string RemoteHost1 = "localhost";
        public static readonly string RemoteHost2 = "localhost";

        public static readonly string TestDirectory = @"\graywulf\test";
    }
}
