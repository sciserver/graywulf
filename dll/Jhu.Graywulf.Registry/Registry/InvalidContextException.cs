/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Represents and exception which is thrown when the entity context is invalid for
    /// the operation being executed.
    /// </summary>
    [Serializable]
    public class InvalidContextException : RegistryException
    {
        public InvalidContextException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public InvalidContextException()
            : base()
        {
        }

        public InvalidContextException(string message)
            : base(message)
        {
        }
    }
}
