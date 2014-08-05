using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    public class AuthRequest
    {
        [DataMember(Name = "auth", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public Auth Auth { get; set; }

        public AuthRequest()
        {
        }
    }
}
