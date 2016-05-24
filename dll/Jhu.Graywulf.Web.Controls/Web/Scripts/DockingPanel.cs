using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Jhu.Graywulf.Web.Scripts
{
    public class DockingPanel : Script
    {
        public override string Name
        {
            get
            {
                return "dockingPanel";
            }
        }

        public override ScriptResourceDefinition Mapping
        {
            get
            {
                return null;
            }
        }

        public override ScriptReference Reference
        {
            get
            {
                return new ScriptReference("Jhu.Graywulf.Web.Controls.DockingPanel.js", "Jhu.Graywulf.Web.Controls");
            }
        }
    }
}
