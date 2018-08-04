using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Extensions.NameResolution
{
    public static class Constants
    {
        public const string SystemVariableNamePartCount = "PARTCOUNT";
        public const string SystemVariableNamePartId = "PARTID";

        public static readonly HashSet<string> GraywulfSystemFunctionNames = new HashSet<string>(Schema.SchemaManager.Comparer)
        {
        };

        public static readonly HashSet<string> GraywulfSystemVariableNames = new HashSet<string>(Schema.SchemaManager.Comparer)
        {
            // Custom system variables
            Constants.SystemVariableNamePartCount,
            Constants.SystemVariableNamePartId,
        };
    }
}
