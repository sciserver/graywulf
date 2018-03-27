using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.AccessControl
{
    [Serializable]
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException()
        {
        }

        public AccessDeniedException(string message)
            : base(message)
        {
        }

        protected AccessDeniedException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
