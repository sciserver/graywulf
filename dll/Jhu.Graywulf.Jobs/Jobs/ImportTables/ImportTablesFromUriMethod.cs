using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Jobs.ImportTables
{
    public class ImportTablesFromUriMethod : ImportTablesMethod
    {
        public override string ID
        {
            get { return "import_uri"; }
        }

        public override string Description
        {
            get { return "Import from URL"; }
        }

        public override void RegisterVirtualPaths(EmbeddedVirtualPathProvider vpp)
        {
            vpp.RegisterVirtualPath(ImportTablesFromUriForm.GetUrl(), GetResourceName(typeof(ImportTablesFromUriForm), ".ascx"));
        }

        public override string GetForm()
        {
            return ImportTablesFromUriForm.GetUrl();
        }
    }
}
