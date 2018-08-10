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
        public const string DefaultOutputTableNamePattern = "Results_" + IO.Constants.CombinedResultsetNameToken;
    }
}
