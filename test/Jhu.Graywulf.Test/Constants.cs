using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Test
{
    public static class Constants
    {
        //public static readonly string TestConnectionString = "Data Source=localhost;Initial Catalog=Graywulf_Test;Integrated Security=true";
        public static readonly string TestConnectionString = "Data Source=sdss3p;Initial Catalog=BESTDR10;Integrated Security=true;;User ID=skyuser;Password=nchips54 ";
        public static readonly string Localhost = Environment.MachineName;

        public static readonly string RemoteHost1 = "localhost";
        public static readonly string RemoteHost2 = "localhost";

        public static readonly string GWCode = @"data\data0\gwcode";
    }
}
