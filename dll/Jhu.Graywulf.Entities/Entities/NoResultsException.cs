using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Entities
{
    [Serializable]
    public class NoResultsException : Exception
    {
        public NoResultsException()
        {
        }

        public NoResultsException(string message)
            : base(message)
        {
        }

        protected NoResultsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
