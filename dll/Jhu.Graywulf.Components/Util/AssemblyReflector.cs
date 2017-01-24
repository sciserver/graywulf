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
