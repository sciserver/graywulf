using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a user.")]
    public class UserResponse
    {
        [DataMember(Name = "user")]
        [Description("A user.")]
        public User User
        { get; set; }

        public UserResponse(User user)
        {
            User = user;
        }
    }
}
