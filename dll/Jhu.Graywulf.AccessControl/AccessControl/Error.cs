using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace Jhu.Graywulf.AccessControl
{
    static class Error
    {
        public static AccessDeniedException AccessDenied()
        {
            return new AccessDeniedException(ErrorMessages.AccessDenied);
        }
    }
}
