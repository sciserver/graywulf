using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Extensions.QueryRewriting
{
    public static class Constants
    {
        public const string PartitionCountParameterName = "@__partCount";
        public const string PartitionIdParameterName = "@__partId";
        public const string PartitionKeyMinParameterName = "@__partKeyMin";
        public const string PartitionKeyMaxParameterName = "@__partKeyMax";

        public static readonly Dictionary<string, string> SystemVariableMappings = new Dictionary<string, string>(Jhu.Graywulf.Sql.Extensions.Parsing.GraywulfSqlParser.ComparerInstance)
        {
            { NameResolution.Constants.SystemVariableNamePartCount, PartitionCountParameterName },
            { NameResolution.Constants.SystemVariableNamePartId, PartitionIdParameterName},
        };
    }
}
