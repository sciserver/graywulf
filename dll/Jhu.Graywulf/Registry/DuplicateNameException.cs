using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Represents and exception which is thrown when an entity with the same name already exists
    /// on the same level of the entity hierarchy.
    /// </summary>
    [Serializable]
    public class DuplicateNameException : RegistryException
    {
        public DuplicateNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Default constructor that initializes the class.
        /// </summary>
        public DuplicateNameException()
            : base()
        {
        }

        /// <summary>
        /// Constructor that sets the error message.
        /// </summary>
        /// <param name="message">Error message.</param>
        public DuplicateNameException(string message)
            : base(message)
        {
        }
    }
}
