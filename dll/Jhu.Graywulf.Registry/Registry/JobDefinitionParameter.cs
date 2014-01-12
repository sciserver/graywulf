using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Registry
{
    [Serializable]
    public class JobDefinitionParameter : Parameter, ICloneable
    {
        private string typeName;
        private JobParameterDirection direction;

        [XmlAttribute]
        [DefaultValue(null)]
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        [XmlAttribute]
        [DefaultValue(JobParameterDirection.Unknown)]
        public JobParameterDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public JobDefinitionParameter()
        {
            InitializeMembers();
        }

        public JobDefinitionParameter(JobDefinitionParameter old)
            :base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.typeName = null;
            this.direction = JobParameterDirection.Unknown;
        }

        private void CopyMembers(JobDefinitionParameter old)
        {
            this.typeName = old.typeName;
            this.direction = old.direction;
        }

        public override object Clone()
        {
            return new JobDefinitionParameter(this);
        }
    }
}
