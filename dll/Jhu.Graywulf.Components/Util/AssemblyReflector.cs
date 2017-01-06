using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jhu.Graywulf.Util
{
    public static class AssemblyReflector
    {
        private static readonly HashSet<string> SystemNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "System",
            "Microsoft",
            "mscorlib",
            "PresentationFramework",
            "WindowsBase",
            "Accessibility",
            "PresentationCore",
            "UIAutomationTypes",
            "UIAutomationProvider",
            "ReachFramework",
            "PresentationUI",
        };

        public static bool IsSystem(Assembly a)
        {
            return IsSystem(a.GetName());
        }

        public static bool IsSystem(AssemblyName an)
        {
            var name = an.Name;
            var idx = name.IndexOf('.');

            if (idx > 0)
            {
                name = name.Substring(0, idx);
            }

            return SystemNames.Contains(name);
        }

        public static Dictionary<string, AssemblyName> GetReferencedAssemblies(Assembly a)
        {
            return GetReferencedAssemblies(a, false);
        }

        public static Dictionary<string, AssemblyName> GetReferencedAssemblies(Assembly a, bool includeSystemAssemblies)
        {
            var names = new Dictionary<string, AssemblyName>();
            GetReferencedAssemblies(a, includeSystemAssemblies, names);
            return names;
        }

        private static void GetReferencedAssemblies(Assembly a, bool includeSystemAssemblies, Dictionary<string, AssemblyName> names)
        {
            foreach (var an in a.GetReferencedAssemblies())
            {
                if (!names.ContainsKey(an.FullName) &&
                    (includeSystemAssemblies || !IsSystem(an)))
                {
                    names.Add(an.FullName, an);

                    var aa = Assembly.Load(an);
                    GetReferencedAssemblies(aa, includeSystemAssemblies, names);
                }
            }
        }

        public static string GetCopyright(Assembly a)
        {
            var cpr = a.GetCustomAttribute<AssemblyCopyrightAttribute>();
            return cpr.Copyright;
        }

        public static string GetCopyright()
        {
            return GetCopyright(Assembly.GetExecutingAssembly());
        }
    }
}
