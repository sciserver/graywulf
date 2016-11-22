using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Api.V1
{
    public static class Constants
    {
        public static readonly Dictionary<JobType, Type> WellKnownJobInterfaces = new Dictionary<JobType, Type>()
        {
            { JobType.Query, typeof(Jhu.Graywulf.Jobs.Query.IQueryJob) },
            { JobType.Export, typeof(Jhu.Graywulf.Jobs.ExportTables.IExportTablesJob) },
            { JobType.Import, typeof(Jhu.Graywulf.Jobs.ImportTables.IImportTablesJob) },
            { JobType.SqlScript, typeof(Jhu.Graywulf.Jobs.SqlScript.ISqlScriptJob) },
        };

        public static readonly Dictionary<JobType, Type> WellKnownJobTypes = new Dictionary<JobType, Type>()
        {
            { JobType.Query, typeof(QueryJob) },
            { JobType.Export, typeof(ExportJob) },
            { JobType.Import, typeof(ImportJob) },
            { JobType.SqlScript, typeof(SqlScriptJob) },
        };
    }
}
