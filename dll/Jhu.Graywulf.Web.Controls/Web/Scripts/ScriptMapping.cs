using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Jhu.Graywulf.Web.Scripts
{
    public class ScriptMapping : Script
    {
        public ScriptResourceDefinition Mapping { get; set; }

        public ScriptReference Reference { get; set; }
    }
}
