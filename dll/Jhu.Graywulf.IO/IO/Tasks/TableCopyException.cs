using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.IO.Tasks
{
    [Serializable]
    public class TableCopyException : Exception
    {
        public TableCopyException()
        {
        }

        public TableCopyException(string message)
            : base(message)
        {
        }
        
        public TableCopyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public TableCopyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
