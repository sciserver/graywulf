using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Jhu.Graywulf.Web.Scripts
{
    public class DockingPanel : ScriptLibrary
    {
        public override Script[] Scripts
        {
            get
            {
                return new[] {
                    new Script()
                    {
                        Name = "dockingPanel",
                        Mapping = null,
                        Reference = new ScriptReference("Jhu.Graywulf.Web.Controls.DockingPanel.js", "Jhu.Graywulf.Web.Controls")
                    }
                };
            }
        }
    }
}
