using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract(Name="queues")]
    public class QueueListResponse
    {
        [DataMember(Name = "queues")]
        public QueueResponse[] Queues { get; set; }

        public QueueListResponse()
        {
        }

        public QueueListResponse(IEnumerable<Queue> queues)
        {
            this.Queues = queues.Select(q => new QueueResponse(q)).ToArray();
        }
    }
}
