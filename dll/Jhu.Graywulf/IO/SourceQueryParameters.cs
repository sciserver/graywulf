using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.IO
{
    [Serializable]
    [DataContract(Namespace="")]
    public class SourceQueryParameters
    {
        private DatasetBase dataset;
        private string query;
        private int timeout;

        [DataMember]
        public DatasetBase Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        [DataMember]
        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        [DataMember]
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public SourceQueryParameters()
            :base()
        {
            InitializeMembers();
        }

        public SourceQueryParameters(SourceQueryParameters old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.dataset = null;
            this.query = null;
            this.timeout = 1200;
        }

        private void CopyMembers(SourceQueryParameters old)
        {
            this.dataset = old.dataset;
            this.query = old.query;
            this.timeout = old.timeout;
        }
    }
}
