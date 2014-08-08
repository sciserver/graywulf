using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a job of a specific type.")]
    public class JobRequest
    {
        [DataMember(Name = "queryJob", EmitDefaultValue=false)]
        [DefaultValue(null)]
        [Description("Conveys a query job.")]
        public QueryJob QueryJob { get; set; }

        [DataMember(Name = "exportJob", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Conveys a data table export job.")]
        public ExportJob ExportJob { get; set; }

        [DataMember(Name = "importJob", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Conveys a data table import job.")]
        public ImportJob ImportJob { get; set; }

        public JobRequest()
        {
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
                throw new ArgumentNullException();  // TODO
            }
        }
    }
}
