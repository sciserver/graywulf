using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents an authentication request.")]
    public class AuthRequest
    {
        [DataMember(Name = "auth", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("User credentials used for authentication.")]
        public Credentials Credentials { get; set; }

        public AuthRequest()
        {
        }
    }
}
