using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract(Name="jobList")]
    public class JobList
    {
        [DataMember(Name = "jobs")]
        public Job[] Jobs { get; set; }

        public JobList()
        {
        }

        public JobList(IEnumerable<Job> jobs)
        {
            this.Jobs = jobs.ToArray();
        }

        public JobList(IEnumerable<Jhu.Graywulf.Registry.JobInstance> jobs)
        {
            this.Jobs = jobs.Select(j => new Job(j)).ToArray();
        }
    }
}
