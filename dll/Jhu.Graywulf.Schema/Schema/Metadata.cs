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
        [NonSerialized]
        private string summary;

        [NonSerialized]
        private string remarks;

        [NonSerialized]
        private string url;

        [NonSerialized]
        private string icon;

        [NonSerialized]
        private string docPage;

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
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        [IgnoreDataMember]
        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        [IgnoreDataMember]
        public string DocPage
        {
            get { return docPage; }
            set { docPage = value; }
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
            this.url = String.Empty;
            this.icon = String.Empty;
            this.docPage = String.Empty;
        }

        private void CopyMembers(Metadata old)
        {
            this.summary = old.summary;
            this.remarks = old.remarks;
            this.url = old.url;
            this.icon = old.icon;
            this.docPage = old.docPage;
        }

    }
}
