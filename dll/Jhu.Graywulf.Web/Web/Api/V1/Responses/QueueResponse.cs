using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    public class QueueResponse
    {
        [DataMember(Name = "queue")]
        public Queue Queue { get; set; }

        public QueueResponse(Queue queue)
        {
            Queue = queue;
        }
    }
}
