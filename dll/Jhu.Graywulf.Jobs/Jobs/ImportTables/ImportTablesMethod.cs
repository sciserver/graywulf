using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Jobs.ImportTables
{
    [Serializable]
    public class ImportTablesMethod
    {
        public string ID { get; set; }

        public string Description { get; set; }

        public Uri BaseUri { get; set; }

        public bool HasCredentials { get; set; }
    }
}
