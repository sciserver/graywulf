using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Represents the base class for all schema library exceptions
    /// </summary>
    [Serializable]
    public abstract class RegistryException : Exception
    {
        public RegistryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Default constructor that initializes the class.
        /// </summary>
        public RegistryException()
            : base()
        {
        }

        /// <summary>
        /// Constructor that sets the error message.
        /// </summary>
        /// <param name="message">Error message.</param>
        public RegistryException(string message)
            : base(message)
        {
        }
    }
}
