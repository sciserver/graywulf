using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Jobs
{
    public static class Constants
    {
        public const string JobParameterUserGuid = "UserGuid";
        public const string JobParameterJobGuid = "JobGuid";

        public const string JobParameterQuery = "Query";
        public const string JobParameterExport = "Parameters";
        public const string JobParameterImport = "Parameters";

        public const string DefaultQuickResultsTableNamePattern = "quickResults";
        public const string DefaultLongResultsTableNamePattern = "Results_" + Jhu.Graywulf.IO.Constants.ResultsetNameToken;
    }
}
