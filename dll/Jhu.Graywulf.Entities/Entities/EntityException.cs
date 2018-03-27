using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

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

        protected EntityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
