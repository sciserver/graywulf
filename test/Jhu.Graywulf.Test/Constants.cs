using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Test
{
    public static class Constants
    {
        public static readonly string TestConnectionString = "Data Source=localhost;Initial Catalog=Graywulf_Test;Integrated Security=true";
        public static readonly string TestConnectionStringMySql = "SERVER=localhost;DATABASE=Graywulf_Test;UID=graywulf;PASSWORD=password;";

        public static readonly string Localhost = Environment.MachineName;

        public static readonly string RemoteHost1 = "localhost";
        public static readonly string RemoteHost2 = "localhost";

        public static readonly string GWCode = @"data\data0\gwcode";
    }
}
