using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public static class Constants
    {
        public const string DefaultQuickResultsTableNamePattern = "QuickResults";
        public const string DefaultLongResultsTableNamePattern = "Results";
        public const string DefaultOutputTableNamePattern = "Output_" + IO.Constants.ResultsetNameToken;

        public const string PartitionCountParameterName = "@__partCount";
        public const string PartitionIdParameterName = "@__partId";
        public const string PartitionKeyMinParameterName = "@__partKeyMin";
        public const string PartitionKeyMaxParameterName = "@__partKeyMax";

        public static readonly Dictionary<string, string> SystemVariableMappings = new Dictionary<string, string>(SqlParser.ComparerInstance)
        {
            { NameResolution.Constants.SystemVariableNamePartCount, PartitionCountParameterName },
            { NameResolution.Constants.SystemVariableNamePartId, PartitionIdParameterName},
        };
    }
}
