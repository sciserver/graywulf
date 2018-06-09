using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a list of user groups.")]
    public class UserGroupListResponse
    {
        [DataMember(Name = "userGroups")]
        [Description("An array of user groups.")]
        public UserGroup[] UserGroups
        { get; set; }
    }
}
