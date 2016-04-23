using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace Jhu.Graywulf.AccessControl
{
    static class Error
    {
        public static SecurityException AccessDenied()
        {
            return new SecurityException(ErrorMessages.AccessDenied);
        }
    }
}
