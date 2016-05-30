using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1.Responses
{
    [DataContract(Name = "userList")]
    [Description("Represents a list of users.")]
    public class UserListResponse
    {
        [DataMember(Name = "users")]
        [Description("An array of users.")]
        public UserResponse[] Users
        { get; set; }
    }
}
