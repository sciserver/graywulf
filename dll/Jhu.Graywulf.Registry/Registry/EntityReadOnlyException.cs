using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Represents and exception which is thrown when an entity is about to be loaded
    /// from the database but not found.
    /// </summary>
    /// <remarks>
    /// This exception is thrown when invalid Guid or fully qualified name is passed to
    /// any of the load functions.
    /// </remarks>
    [Serializable]
    public class EntityReadOnlyException : RegistryException
    {
        public EntityReadOnlyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public EntityReadOnlyException()
            : base()
        {
        }

        public EntityReadOnlyException(string message)
            : base(message)
        {
        }
    }
}
