using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Services
{
    [DataContract]
    [Description("Represents a detailed error message.")]
    public class RestErrorResponse
    {
        [DataMember(Name = "restError")]
        [Description("A detailed error message")]
        public RestError RestError { get; set; }

        public RestErrorResponse()
        {
        }
    }
}
