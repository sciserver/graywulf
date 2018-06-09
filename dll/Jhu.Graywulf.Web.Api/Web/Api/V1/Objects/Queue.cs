using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a job execution queue.")]
    public class Queue
    {
        internal Guid Guid { get; set; }

        [DataMember(Name="name")]
        [Description("Name of the queue.")]
        public string Name { get; set; }

        [DataMember(Name = "pendingJobCount")]
        [Description("Number of jobs pending in the queue including those of other users.")]
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
