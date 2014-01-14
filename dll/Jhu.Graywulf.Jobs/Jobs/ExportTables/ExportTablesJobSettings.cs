using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    public class ExportTablesJobSettings : ParameterCollection
    {
        public string OutputDirectory
        {
            get { return GetValue<string>("OutputDirectory"); }
            set { SetValue("OutputDirectory", value); }
        }

        public ExportTablesJobSettings()
        {
        }

        public ExportTablesJobSettings(ParameterCollection old)
            : base(old)
        {
        }
    }
}
