using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Jhu.Graywulf.Web.Scripts
{
    public class JQuery : Script
    {
        public override string Name
        {
            get
            {
                return "jquery";
            }
        }

        public override ScriptResourceDefinition Mapping
        {
            get
            {
                return new ScriptResourceDefinition
                {
                    Path = "~/Scripts/jquery-2.2.3.min.js",
                    DebugPath = "~/Scripts/jquery-2.2.3.min.js",
                    CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.2.3.min.js",
                    CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.2.3.js",
                };
            }
        }
    }
}
