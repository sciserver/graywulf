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

        [DataMember(Name = "value")]
        [Description("Value of the item.")]
        public TestEnum Value { get; set; }

        [DataMember(Name = "moreValues")]
        [Description("More values of the item.")]
        public TestEnum[] MoreValues { get; set; }

        [DataMember(Name = "moreItems")]
        [Description("More child items of the item.")]
        public TestItemCollection MoreItems { get; set; }
    }
}
