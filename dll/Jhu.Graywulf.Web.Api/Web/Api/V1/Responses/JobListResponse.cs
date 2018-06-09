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
    [Description("Represents a list of jobs.")]
    public class JobListResponse
    {
        [DataMember(Name = "jobs")]
        [Description("An array of jobs.")]
        public JobResponse[] Jobs { get; set; }

        public JobListResponse()
        {
        }

        public JobListResponse(IEnumerable<Job> jobs)
        {
            this.Jobs = jobs.Select(j => new JobResponse(j)).ToArray();
        }

        public JobListResponse(IEnumerable<Jhu.Graywulf.Registry.JobInstance> jobs)
        {
            this.Jobs = jobs.Select(j => new JobResponse(JobFactory.CreateJobFromInstance(j))).ToArray();
        }
    }
}
