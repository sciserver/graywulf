using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities
{
    [Serializable]
    public class EntityException : Exception
    {
        public EntityException()
        {
        }

        public EntityException(string message)
            : base(message)
        {
        }
    }
}
