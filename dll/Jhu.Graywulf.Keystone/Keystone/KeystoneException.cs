using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Jhu.Graywulf.SimpleRestClient;

namespace Jhu.Graywulf.Keystone
{
    [Serializable]
    public class KeystoneException : Exception
    {
        private string title;
        private HttpStatusCode statusCode;

        public string Title
        {
            get { return title; }
            internal set { title = value; }
        }

        public HttpStatusCode StatusCode
        {
            get { return statusCode; }
            internal set { statusCode = value; }
        }

        internal KeystoneException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
