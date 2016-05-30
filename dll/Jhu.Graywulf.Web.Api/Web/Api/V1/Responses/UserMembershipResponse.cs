using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a user's membership within a group with a role.")]
    public class UserMembershipResponse
    {
        [DataMember(Name = "role")]
        [Description("A user role.")]
        public UserMembership Role
        { get; set; }

        public UserMembershipResponse(UserMembership role)
        {
            Role = role;
        }
    }
}
