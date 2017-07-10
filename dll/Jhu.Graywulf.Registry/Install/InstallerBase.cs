using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public abstract class InstallerBase : ContextObject
    {
        protected InstallerBase()
        {
        }

        protected InstallerBase(RegistryContext context)
            : base(context)
        {
        }

        protected string GetUnversionedTypeName(Type type)
        {
            return Util.TypeNameFormatter.ToUnversionedAssemblyQualifiedName(type);
        }
    }
}
