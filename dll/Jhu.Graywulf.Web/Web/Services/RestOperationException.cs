using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Services
{
    [Serializable]
    public class RestOperationException : Exception
    {
        private RestError restError;

        public RestError RestError
        {
            get { return restError; }
        }

        public RestOperationException(Exception innerException, string logEventID)
            :base (innerException.Message, innerException)
        {
            this.restError = new RestError(innerException, logEventID);
        }
    }
}
