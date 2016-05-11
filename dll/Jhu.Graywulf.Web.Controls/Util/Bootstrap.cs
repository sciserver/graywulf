using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Jhu.Graywulf.Util
{
    public static class Bootstrap
    {

        //public const string BootstrapUrl = ""http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.js"";
        public const string BootstrapUrl = "http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js";

        public static void Register(ScriptManager scriptManager)
        {
            // Check if JQuery has been added before
            if (scriptManager.Scripts.FirstOrDefault(s => s.Path == BootstrapUrl) != null)
            {
                return;
            }

            var sr = new ScriptReference(BootstrapUrl);
            scriptManager.Scripts.Add(sr);
        }
    }
}
