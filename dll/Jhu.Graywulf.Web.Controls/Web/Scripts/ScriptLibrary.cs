using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Jhu.Graywulf.Web.Scripts
{
    public abstract class ScriptLibrary
    {
        public abstract Script[] Scripts { get; }

        public static void RegisterMappings(ScriptLibrary scriptlib)
        {
            var scripts = scriptlib.Scripts;

            for (int i = 0; i < scripts.Length; i++)
            {
                var mapping = scripts[i].Mapping;

                if (mapping != null)
                {
                    ScriptManager.ScriptResourceMapping.AddDefinition(scripts[i].Name, mapping);
                }
            }
        }

        public static void RegisterReferences(ScriptManager scriptManager, ScriptLibrary scriptlib)
        {
            var scripts = scriptlib.Scripts;

            for (int i = 0; i < scripts.Length; i++)
            {
                if (scriptManager.Scripts.FirstOrDefault(s => s.Name == scripts[i].Name) != null)
                {
                    continue;
                }

                var reference = scripts[i].Reference;

                if (reference == null)
                {
                    reference = new ScriptReference()
                    {
                        Name = scripts[i].Name
                    };
                }

                scriptManager.Scripts.Add(reference);
            }
        }
    }
}
