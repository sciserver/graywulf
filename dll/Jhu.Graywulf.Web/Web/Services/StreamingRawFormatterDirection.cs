using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Services
{
    [Flags]
    public enum StreamingRawFormatterDirection
    {
        None = 0,
        Parameters = 1,
        ReturnValue = 2,
        Both = Parameters | ReturnValue
    }
}
