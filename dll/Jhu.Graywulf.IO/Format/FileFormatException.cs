using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    public class FileFormatException : Exception
    {
        public FileFormatException()
        {
        }

        public FileFormatException(string message)
            : base(message)
        {
        }

        protected FileFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
