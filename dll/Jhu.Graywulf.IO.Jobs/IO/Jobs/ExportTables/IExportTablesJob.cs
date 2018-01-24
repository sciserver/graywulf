using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.IO.Jobs.ExportTables
{
    public interface IExportTablesJob : IJob
    {
        InArgument<ExportTablesParameters> Parameters { get; set; }
    }
}
