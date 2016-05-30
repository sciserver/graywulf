using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name = "groupList")]
    [Description("Represents a list of user groups.")]
    public class UserGroupListResponse
    {
        [DataMember(Name = "groups")]
        [Description("An array of user groups.")]
        public UserGroupResponse[] UserGroups
        { get; set; }
    }
}
