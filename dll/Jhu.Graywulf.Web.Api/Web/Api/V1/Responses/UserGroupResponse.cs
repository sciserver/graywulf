using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a user group.")]
    public class UserGroupResponse
    {
        [DataMember(Name = "group")]
        [Description("A user group.")]
        public UserGroup UserGroup
        { get; set; }

        public UserGroupResponse(UserGroup group)
        {
            UserGroup = group;
        }
    }
}
