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
            { JobType.Query, typeof(Jhu.Graywulf.Sql.Jobs.Query.IQueryJob) },
            { JobType.Export, typeof(Jhu.Graywulf.IO.Jobs.ExportTables.IExportTablesJob) },
            { JobType.Import, typeof(Jhu.Graywulf.IO.Jobs.ImportTables.IImportTablesJob) },
            { JobType.Copy, typeof(Jhu.Graywulf.IO.Jobs.CopyTables.ICopyTablesJob) },
            { JobType.SqlScript, typeof(Jhu.Graywulf.Sql.Jobs.SqlScript.ISqlScriptJob) },
        };

        public static readonly Dictionary<JobType, Type> WellKnownJobTypes = new Dictionary<JobType, Type>()
        {
            { JobType.Query, typeof(QueryJob) },
            { JobType.Export, typeof(ExportJob) },
            { JobType.Import, typeof(ImportJob) },
            { JobType.Copy, typeof(CopyJob) },
            { JobType.SqlScript, typeof(SqlScriptJob) },
        };
    }
}
