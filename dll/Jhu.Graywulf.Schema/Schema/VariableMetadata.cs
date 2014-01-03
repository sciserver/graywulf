using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class VariableMetadata
    {
        [NonSerialized]
        private string summary;

        [NonSerialized]
        private string unit;

        [NonSerialized]
        private string content;

        [NonSerialized]
        private string format;

        [DataMember]
        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }

        [DataMember]
        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        [DataMember]
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        [DataMember]
        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        public VariableMetadata()
        {
            InitializeMembers();
        }

        public VariableMetadata(VariableMetadata old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.summary = String.Empty;
            this.unit = String.Empty;
            this.content = String.Empty;
            this.format = "{0}";
        }

        private void CopyMembers(VariableMetadata old)
        {
            this.summary = old.summary;
            this.unit = old.unit;
            this.content = old.content;
            this.format = old.format;
        }
    }
}
