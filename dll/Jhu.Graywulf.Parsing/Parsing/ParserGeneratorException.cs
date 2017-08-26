using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    [Serializable]
    public class ParserGeneratorException : Exception
    {
        public ParserGeneratorException()
            : base()
        {
        }

        public ParserGeneratorException(string message)
            : base(message)
        {
        }

        public ParserGeneratorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ParserGeneratorException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            :base(info, context)
        {
        }
    }
}
