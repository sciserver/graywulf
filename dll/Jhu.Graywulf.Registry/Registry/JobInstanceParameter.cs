using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Registry
{
    [Serializable]
    public class JobInstanceParameter : Parameter, ICloneable
    {
        private JobParameterDirection direction;

        [XmlAttribute]
        [DefaultValue(JobParameterDirection.Unknown)]
        public JobParameterDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public JobInstanceParameter()
        {
            InitializeMembers();
        }

        public JobInstanceParameter(JobInstanceParameter old)
            :base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.direction = JobParameterDirection.Unknown;
        }

        private void CopyMembers(JobInstanceParameter old)
        {
            this.direction = old.direction;
        }

        public override object Clone()
        {
            return new JobInstanceParameter(this);
        }
    }
}
