using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    static class Error
    {
        public static EntityNotFoundException EntityNotFound(string name)
        {
            return new EntityNotFoundException(String.Format(ExceptionMessages.EntityNotFound, name));
        }

        public static EntityNotFoundException EntityNotFound(Guid guid)
        {
            return new EntityNotFoundException(String.Format(ExceptionMessages.EntityNotFound, guid.ToString()));
        }
    }
}
