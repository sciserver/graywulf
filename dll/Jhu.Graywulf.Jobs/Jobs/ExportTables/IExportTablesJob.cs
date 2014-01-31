using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    public interface IExportTablesJob
    {
        InArgument<ExportTablesParameters> Parameters { get; set; }
    }
}
