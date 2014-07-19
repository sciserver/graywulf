using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SimpleRestClient
{
    [Serializable]
    public class RestException : Exception
    {
        private string body;

        public string Body
        {
            get { return body; }
            internal set { body = value; }
        }

        public RestException()
        {
        }

        public RestException(string message)
            :base(message)
        {
        }

        public RestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
