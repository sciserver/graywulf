using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Logging
{
    [Serializable]
    public class LoggingException : Exception
    {
        public LoggingException()
        {
        }

        public LoggingException(string message)
            : base(message)
        {
        }

        public LoggingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
