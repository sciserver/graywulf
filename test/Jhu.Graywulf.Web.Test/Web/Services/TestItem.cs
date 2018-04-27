using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Services
{
    [DataContract]
    [Description("Represents an item.")]
    public class TestItem
    {
        [DataMember(Name = "id")]
        [Description("Unique identifier of the item.")]
        public Guid Guid { get; set; }

        [DataMember(Name = "name")]
        [Description("Name of the item.")]
        public string Name { get; set; }
    }
}
