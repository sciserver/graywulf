using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.IO.Jobs.ExportTables
{
    public interface IExportTablesForm
    {
        Uri Uri { get; set; }

        Uri CustomizableUri { get; set; }

        Credentials Credentials { get; set; }

        void GenerateDefaultUri(string filename);
    }
}
