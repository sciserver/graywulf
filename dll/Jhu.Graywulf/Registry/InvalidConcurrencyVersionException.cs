using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Represents and exception which is thrown when an entity concurrency version collision occured.
    /// </summary>
    /// <remarks>
    /// In order to ensure consistency of the cluster schema, concurrent processes can only modify
    /// an entity when they have a fresh copy of it. Every time an entity is modified gets a new
    /// concurrency version number. Just before modification the library makes sure that the
    /// concurrency version is the same in the database and the object.
    /// </remarks>
    [Serializable]
    public class InvalidConcurrencyVersionException : RegistryException
    {
        public InvalidConcurrencyVersionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public InvalidConcurrencyVersionException()
            : base()
        {
        }

        public InvalidConcurrencyVersionException(string message)
            : base(message)
        {
        }
    }
}
