using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name = "group")]
    [Description("Represents a user group")]
    public class UserGroup
    {
        [DataMember(Name = "name")]
        [Description("User group name.")]
        public string Name
        { get; set; }
    }
}
