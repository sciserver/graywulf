using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Security
{
    [Flags]
    public enum AuthenticatorProtocolType : int
    {
        WebInteractive = 1,
        WebRequest = 2,
        RestRequest = 4
    }
}
