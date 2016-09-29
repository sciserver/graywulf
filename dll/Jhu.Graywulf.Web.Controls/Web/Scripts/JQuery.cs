using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Jhu.Graywulf.Web.Scripts
{
    public class JQuery : ScriptLibrary
    {
        public override Script[] Scripts
        {
            get
            {
                return new[] {
                    new Script()
                    {
                        Name = "jquery",
                        Mapping = new ScriptResourceDefinition()
                        {
                            Path = "~/Scripts/jQuery/jquery-2.2.4.min.js",
                            DebugPath = "~/Scripts/jQuery/jquery-2.2.4.js",
                            CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.2.4.min.js",
                            CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.2.4.js",
                        }
                    }
                };
            }
        }
    }
}
