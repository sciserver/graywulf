using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Security
{
    [Serializable]
    public class IdentityProviderException : Exception
    {
        public IdentityProviderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Default constructor that initializes the class.
        /// </summary>
        public IdentityProviderException()
            : base()
        {
        }

        /// <summary>
        /// Constructor that sets the error message.
        /// </summary>
        /// <param name="message">Error message.</param>
        public IdentityProviderException(string message)
            : base(message)
        {
        }

        public IdentityProviderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
