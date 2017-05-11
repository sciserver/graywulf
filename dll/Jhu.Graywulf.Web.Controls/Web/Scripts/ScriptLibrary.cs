using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Jhu.Graywulf.Web.Scripts
{
    public abstract class ScriptLibrary
    {
        public abstract Script[] Scripts { get; }
        public virtual string[] StyleSheets { get; } 

        public static void Register(ScriptManager scriptManager, ScriptLibrary scriptlib)
        {
            RegisterStyleSheets(scriptManager.Page, scriptlib);
            RegisterScriptBlocks(scriptManager.Page, scriptlib);
            RegisterReferences(scriptManager, scriptlib);
        }

        public static void RegisterStyleSheets(Page page, ScriptLibrary scriptlib)
        {
            if (scriptlib.StyleSheets != null)
            {
                for (int i = 0; i < scriptlib.StyleSheets.Length; i++)
                {
                    var link = new HtmlLink();
                    link.Href = scriptlib.StyleSheets[i];
                    link.Attributes["rel"] = "stylesheet";

                    page.Header.Controls.Add(link);
                }
            }
        }

        private static void RegisterScriptBlocks(Page page, ScriptLibrary scriptlib)
        {
            var scripts = scriptlib.Scripts;

            for (int i = 0; i < scripts.Length; i++)
            {
                if (scripts[i] is ScriptBlock)
                {
                    var code = ((ScriptBlock)scripts[i]).Code;

                    if (code != null && !page.ClientScript.IsStartupScriptRegistered(scriptlib.GetType(), scripts[i].Name))
                    {
                        page.ClientScript.RegisterStartupScript(scriptlib.GetType(), scripts[i].Name, code, true);
                    }
                }
            }
        }

        public static void RegisterMappings(ScriptLibrary scriptlib)
        {
            var scripts = scriptlib.Scripts;

            for (int i = 0; i < scripts.Length; i++)
            {
                if (scripts[i] is ScriptMapping)
                {
                    var mapping = ((ScriptMapping)scripts[i]).Mapping;

                    if (mapping != null)
                    {
                        ScriptManager.ScriptResourceMapping.AddDefinition(scripts[i].Name, mapping);
                    }
                }
            }
        }

        private static void RegisterReferences(ScriptManager scriptManager, ScriptLibrary scriptlib)
        {
            var scripts = scriptlib.Scripts;

            for (int i = 0; i < scripts.Length; i++)
            {
                if (scripts[i] is ScriptMapping)
                {
                    if (scriptManager.Scripts.FirstOrDefault(s => s.Name == scripts[i].Name) != null)
                    {
                        continue;
                    }

                    var reference = ((ScriptMapping)scripts[i]).Reference;

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
}
