using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Services
{
    [DataContract]
    public class RestError
    {
        [IgnoreDataMember]
        public Exception InnerException { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string LogEventID { get; set; }

        public RestError()
        {
        }

        public RestError(Exception innerException, string logEventID)
        {
            this.InnerException = innerException;
            this.Type = innerException.GetType().FullName;
            this.Message = innerException.Message;
            this.StackTrace = innerException.StackTrace;
            this.LogEventID = logEventID;
        }

        public RestError(Exception innerException)
            : this(innerException, null)
        {
            // Overload
        }
    }
}
