using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a list of job queues.")]
    public class QueueListResponse
    {
        [DataMember(Name = "queues")]
        [Description("An array of job queues.")]
        public Queue[] Queues { get; set; }

        public QueueListResponse()
        {
        }

        public QueueListResponse(IEnumerable<Queue> queues)
        {
            this.Queues = queues.ToArray();
        }
    }
}
