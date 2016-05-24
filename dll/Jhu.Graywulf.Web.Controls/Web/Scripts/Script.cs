using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Jhu.Graywulf.Web.Scripts
{
    public abstract class Script
    {
        public abstract string Name
        { get; }

        public abstract ScriptResourceDefinition Mapping
        { get; }

        public virtual ScriptReference Reference
        {
            get
            {
                return new ScriptReference()
                {
                    Name = Name
                };
            }
        }

        public static void RegisterMapping(Script script)
        {
            ScriptManager.ScriptResourceMapping.AddDefinition(script.Name, script.Mapping);
        }

        public static void Register(ScriptManager scriptManager, Script script)
        {
            // Check if JQuery has been added before
            if (scriptManager.Scripts.FirstOrDefault(s => s.Name == script.Name) != null)
            {
                return;
            }

            scriptManager.Scripts.Add(script.Reference);
        }
    }
}
