using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class Metadata
    {
        private string summary;
        private string remarks;

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

        public Metadata()
        {
            InitializeMembers();
        }

        public Metadata(Metadata old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.summary = String.Empty;
            this.remarks = String.Empty;
        }

        private void CopyMembers(Metadata old)
        {
            this.summary = old.summary;
            this.remarks = old.remarks;
        }

    }
}
