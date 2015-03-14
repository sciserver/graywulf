using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class PluginBase
    {
        public abstract string ID { get; }

        public abstract string Description { get; }

        protected string GetResourceName(Type type, string extension)
        {
            var cname = type.FullName;
            var aname = type.Assembly.GetName().Name;

            return cname + extension + ", " + aname;
        }

        public abstract void RegisterVirtualPaths(EmbeddedVirtualPathProvider vpp);

        public abstract string GetForm();
    }
}
