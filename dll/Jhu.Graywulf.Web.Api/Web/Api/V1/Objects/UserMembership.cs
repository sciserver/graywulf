using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Role of a user within a group.")]
    public class UserMembership
    {
        [DataMember(Name = "user")]
        [Description("User name.")]
        public string UserName
        { get; set; }

        [DataMember(Name = "group")]
        [Description("Group name.")]
        public string GroupName
        { get; set; }

        [DataMember(Name = "role")]
        [Description("Role.")]
        public string RoleName
        { get; set; }
    }
}
