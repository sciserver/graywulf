using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Jhu.Graywulf.Util
{
    public static class JQuery
    {
        //public const string JQueryUrl = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.0.js";
        public const string JQueryUrl = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.12.0.min.js";
        //public const string JQueryUrl = "http://code.jquery.com/jquery-1.8.3.min.js";

        public static void Register(ScriptManager scriptManager)
        {
            // Check if JQuery has been added before
            if (scriptManager.Scripts.FirstOrDefault(s => s.Path == JQueryUrl) != null)
            {
                return;
            }

            var sr = new ScriptReference(JQueryUrl);
            scriptManager.Scripts.Add(sr);
        }
    }
}
