using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Services
{
    [Serializable]
    public class ResourceNotFoundException : Exception
    {
        // TODO: consider using exception from entities instead

        public ResourceNotFoundException()
        {
        }

        public ResourceNotFoundException(string message)
            : base(message)
        {
        }
    }
}
