using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace="")]
    public class DatabaseObjectMetadata
    {
        [NonSerialized]
        private string summary;

        [NonSerialized]
        private string remarks;

        [NonSerialized]
        private string example;

        [DataMember]
        public string Summary 
        {
            get { return summary; }
            set { summary = value; }
        }

        [DataMember]
        public string Remarks 
        {
            get { return remarks; }
            set { remarks = value; }
        }

        [DataMember]
        public string Example
        {
            get { return example; }
            set { example = value; }
        }

        public DatabaseObjectMetadata()
        {
            InitializeMembers();
        }

        public DatabaseObjectMetadata(DatabaseObjectMetadata old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.summary = String.Empty;
            this.remarks = String.Empty;
            this.example = String.Empty;
        }

        private void CopyMembers(DatabaseObjectMetadata old)
        {
            this.summary = old.summary;
            this.remarks = old.remarks;
            this.example = old.example;
        }
    }
}
