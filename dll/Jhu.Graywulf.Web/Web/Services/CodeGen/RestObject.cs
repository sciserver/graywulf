using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public abstract class RestObject
    {
        public abstract void SubstituteTokens(StringBuilder script);
    }
}
