using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.CommandLineParser
{
    [Serializable]
    public class ArgumentParserException : Exception
    {
        public ArgumentParserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ArgumentParserException()
            : base()
        {
        }

        public ArgumentParserException(string message)
            : base(message)
        {
        }

        public ArgumentParserException(string message, Exception innerException)
            :base(message, innerException)
        {
        }
    }
}
