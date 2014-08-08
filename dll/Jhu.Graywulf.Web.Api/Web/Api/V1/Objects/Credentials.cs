using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name = "credentials")]
    [Description("Represents user credentials.")]
    public class Credentials
    {
        [DataMember(Name = "username", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Username")]
        public string Username { get; set; }

        [DataMember(Name = "password", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Password in clear text.")]
        public string Password { get; set; }

        [DataMember(Name = "headers", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("A collection of headers used for authentication.")]
        public Headers Headers { get; set; }

        public Credentials()
        {
        }

        public Credentials(Jhu.Graywulf.IO.Credentials credentials)
        {
            this.Username = credentials.UserName;
            this.Password = credentials.Password;
            this.Headers = new Headers(credentials.AuthenticationHeaders);
        }
    }
}
