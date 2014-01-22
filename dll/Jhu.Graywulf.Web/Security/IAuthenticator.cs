using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Security
{
    public interface IAuthenticator
    {
        string Protocol { get; }
        string Authority { get; }
        string DisplayName { get; }
    }
}
