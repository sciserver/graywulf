using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract(Name="queueList")]
    public class QueueList
    {
        [DataMember(Name = "queues")]
        public Queue[] Queues { get; set; }

        public QueueList()
        {
        }

        public QueueList(IEnumerable<Queue> queues)
        {
            this.Queues = queues.ToArray();
        }
    }
}
