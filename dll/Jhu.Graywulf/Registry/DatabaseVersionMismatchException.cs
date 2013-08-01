using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements an exception that is thrown when a Database Version mismatch occures
    /// when deploying distributed partitioned views.
    /// </summary>
    [Serializable]
    public class DatabaseVersionMismatchException : DeployException
    {
        public DatabaseVersionMismatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Default contructor.
        /// </summary>
        public DatabaseVersionMismatchException()
            : base()
        {
        }

        /// <summary>
        /// Constructor for initializing the error message.
        /// </summary>
        /// <param name="message">Error message.</param>
        public DatabaseVersionMismatchException(string message)
            : base(message)
        {
        }
    }
}
