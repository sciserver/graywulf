using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.IO.Jobs.ImportTables
{
    public class ImportTablesJobSettings : ParameterCollection
    {
        public ImportTablesJobSettings()
        {
        }

        public ImportTablesJobSettings(ParameterCollection old)
            : base(old)
        {
        }
    }
}
