using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    [Serializable]
    public class RestProxyGeneratorException : Exception
    {
        public RestProxyGeneratorException()
        {
        }

        public RestProxyGeneratorException(string message)
            : base(message)
        {
        }

        public RestProxyGeneratorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RestProxyGeneratorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
