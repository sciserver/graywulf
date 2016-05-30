using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name = "roleList")]
    [Description("Represents a list of user roles within a group.")]
    public class UserMembershipListResponse
    {
        [DataMember(Name = "roles")]
        [Description("An array of user roles")]
        public UserMembershipResponse[] UserRoles
        { get; set; }
    }
}
