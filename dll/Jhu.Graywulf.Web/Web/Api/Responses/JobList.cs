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
        public JobItem[] Jobs { get; set; }

        public JobList()
        {
        }

        public JobList(IEnumerable<Job> jobs)
        {
            this.Jobs = jobs.Select(j => new JobItem(j)).ToArray();
        }

        public JobList(IEnumerable<Jhu.Graywulf.Registry.JobInstance> jobs)
        {
            this.Jobs = jobs.Select(j => new JobItem(new Job(j))).ToArray();
        }
    }
}
