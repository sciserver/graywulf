using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a user.")]

    public class User
    {
        [DataMember(Name = "name")]
        [Description("User name.")]
        public string Name
        { get; set; }

        [DataMember(Name = "isAuthenticated")]
        [Description("User is authenticated")]
        public bool IsAuthenticated
        { get; set; }

        public static User Guest
        {
            get
            {
                return new User()
                {
                    Name = "guest",
                    IsAuthenticated = false
                };
            }
        }

        public User()
        {
        }

        public User(Graywulf.AccessControl.GraywulfIdentity identity)
        {
            Name = identity.Name;
            IsAuthenticated = identity.IsAuthenticated;
        }

    }
}
