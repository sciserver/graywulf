using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements an exception that is thrown when an SMO based deployment function fails.
    /// </summary>
    [Serializable]
    public class DeployException : Exception
    {
        public DeployException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Default contructor.
        /// </summary>
        public DeployException()
            :base()
        {
        }

        /// <summary>
        /// Constructor for initializing the error message.
        /// </summary>
        /// <param name="message">Error message.</param>
        public DeployException(string message)
            : base(message)
        {
        }
    }
}
