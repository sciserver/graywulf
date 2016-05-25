using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name="user")]
    [Description("Represents a user.")]

    public class User
    {
        [DataMember(Name = "name")]
        [Description("User name.")]
        public string Name { get; set; }

        public User()
        {
        }

        public User(Graywulf.AccessControl.GraywulfIdentity identity)
        {
            Name = identity.Name;
        }

    }
}
