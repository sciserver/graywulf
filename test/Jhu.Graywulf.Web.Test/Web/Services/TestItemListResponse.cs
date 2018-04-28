using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Services
{
    [DataContract(Name = "TestItemList")]
    [Description("Represents a list of items.")]
    public class TestItemListResponse
    {
        [DataMember(Name = "items")]
        [Description("An array of items.")]
        public IEnumerable<TestItem> Items { get; set; }
    }
}
