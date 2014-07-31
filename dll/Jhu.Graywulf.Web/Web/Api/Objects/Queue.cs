using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract(Name="queue")]
    public class Queue
    {
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name = "jobCount")]
        public int JobCount { get; set; }

        public Queue(Jhu.Graywulf.Registry.QueueInstance queue)
        {
            this.Name = queue.Name;
            // TODO: implement this
            this.JobCount = 0;
        }
    }
}
