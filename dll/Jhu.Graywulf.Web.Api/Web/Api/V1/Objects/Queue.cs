using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name="queue")]
    public class Queue
    {
        internal Guid Guid { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name = "pendingJobCount")]
        public int PendingJobCount { get; set; }

        public Queue()
        {
        }

        public Queue(Jhu.Graywulf.Registry.QueueInstance queue)
        {
            this.Guid = queue.Guid;
            this.Name = queue.Name;
            // TODO: implement this
            this.PendingJobCount = 0;
        }
    }
}
