using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.IO.Jobs.ImportTables
{
    public interface IImportTablesForm
    {
        Uri Uri { get; set; }

        Credentials Credentials { get; set; }
    }
}
