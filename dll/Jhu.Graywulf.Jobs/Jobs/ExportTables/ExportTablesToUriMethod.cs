using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    public class ExportTablesToUriMethod : ExportTablesMethod
    {
        public override string ID
        {
            get { return "export_uri"; }
        }

        public override string Description
        {
            get { return "Export to URI"; }
        }

        public override void RegisterVirtualPaths(EmbeddedVirtualPathProvider vpp)
        {
            vpp.RegisterVirtualPath(ExportTablesToUriForm.GetUrl(), GetResourceName(typeof(ExportTablesToUriForm), ".ascx"));
        }

        public override string GetForm()
        {
            return ExportTablesToUriForm.GetUrl();
        }
    }
}
