using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1.Objects
{
    [DataContract(Name = "credentials")]
    public class Credentials
    {
        [DataMember(Name = "user", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string User { get; set; }

        [DataMember(Name = "password", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string Password { get; set; }

        [DataMember(Name = "headers", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public Headers Headers { get; set; }

        public Credentials()
        {
        }

        public Credentials(Jhu.Graywulf.IO.Credentials credentials)
        {
            this.User = credentials.UserName;
            this.Password = credentials.Password;
            this.Headers = new Headers(credentials.AuthenticationHeaders);
        }
    }
}
