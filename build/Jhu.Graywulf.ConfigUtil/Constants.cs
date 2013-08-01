using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ConfigUtil
{
    class Constants
    {
        public static readonly Dictionary<ProjectType, string> OriginalConfigFileNames = new Dictionary<ProjectType, string>()
        {
            { ProjectType.Console, "App.original.config" },
            { ProjectType.Web, "Web.original.config" },
        };

        public static readonly Dictionary<ProjectType, string> TargetConfigFileNames = new Dictionary<ProjectType, string>()
        {
            { ProjectType.Console, "App.config" },
            { ProjectType.Web, "Web.config" },
        };
    }
}
