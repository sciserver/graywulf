using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    [Flags]
    public enum RawMessageFormatterDirection
    {
        None = 0,
        ParameterIn = 1,
        ParameterOut = 2,
        ReturnValue = 4,
        All = ParameterIn | ParameterOut | ReturnValue
    }
}
