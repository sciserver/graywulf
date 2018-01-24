using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Sql.Schema
{
    /// <summary>
    /// Thrown whenever an error during schema reflection occures
    /// </summary>
    [Serializable]
    public class SchemaException : Exception
    {
        public SchemaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public SchemaException()
            : base()
        {
        }

        public SchemaException(string message)
            : base(message)
        {
        }

        public SchemaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
