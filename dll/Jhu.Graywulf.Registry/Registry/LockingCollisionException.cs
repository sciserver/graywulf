using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Represents and exception which is thrown when an entity locking collision occured.
    /// </summary>
    /// <remarks>
    /// Processes can put exclusive write locks on entities. When a locking collision occures,
    /// that is another process wants to write the same entity which has a long term lock on it
    /// this exception is thrown.
    /// </remarks>
    [Serializable]
    public class LockingCollisionException : RegistryException
    {
        public LockingCollisionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public LockingCollisionException()
            : base()
        {
        }

        public LockingCollisionException(string message)
            : base(message)
        {
        }
    }
}
