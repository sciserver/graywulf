using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    public class JobResponse
    {
        [DataMember(Name = "queryJob", EmitDefaultValue=false)]
        [DefaultValue(null)]
        public QueryJob QueryJob { get; set; }

        [DataMember(Name = "exportJob", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public ExportJob ExportJob { get; set; }

        [DataMember(Name = "importJob", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public ImportJob ImportJob { get; set; }

        [DataMember(Name = "job", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public Job Job { get; set; }

        public JobResponse(Job job)
        {
            SetValue((dynamic)job);
        }

        private void SetValue(QueryJob job)
        {
            QueryJob = job;
        }

        private void SetValue(ExportJob job)
        {
            ExportJob = job;
        }

        private void SetValue(ImportJob job)
        {
            ImportJob = job;
        }

        private void SetValue(Job job)
        {
            Job = job;
        }

        public Job GetValue()
        {
            if (QueryJob != null)
            {
                return QueryJob;
            }
            else if (ExportJob != null)
            {
                return ExportJob; 
            }
            else if (ImportJob != null)
            {
                return ImportJob;
            }
            else
            {
                return Job;
            }
        }
    }
}
