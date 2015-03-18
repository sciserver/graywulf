using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    public interface IExportTablesForm
    {
        Uri Uri { get; set; }

        Credentials Credentials { get; set; }
    }
}
