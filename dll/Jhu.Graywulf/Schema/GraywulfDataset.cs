using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace="")]
    public class GraywulfDataset : SqlServerDataset, ICloneable
    {
        protected string databaseDefinitionName;
        protected string databaseInstanceName;

        [DataMember]
        public string DatabaseDefinitionName
        {
            get { return databaseDefinitionName; }
            set { databaseDefinitionName = value; }
        }

        [DataMember]
        public string DatabaseInstanceName
        {
            get { return databaseInstanceName; }
            set { databaseInstanceName = value; }
        }

        [IgnoreDataMember]
        public bool IsSpecificInstanceRequired
        {
            get { return !String.IsNullOrWhiteSpace(databaseInstanceName); }
        }

        [IgnoreDataMember]
        public override string FullyQualifiedName
        {
            get
            {
                SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
                return String.Format("[{0}]", csb.InitialCatalog);
            }
        }

        public GraywulfDataset()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public GraywulfDataset(DatasetBase old)
            : base(old)
        {
            InitializeMembers(new StreamingContext());
        }

        public GraywulfDataset(GraywulfDataset old)
            :base(old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.connectionString = String.Empty;

            this.databaseDefinitionName = String.Empty;
            this.databaseInstanceName = String.Empty;
        }

        private void CopyMembers(GraywulfDataset old)
        {
            this.connectionString = old.connectionString;

            this.databaseDefinitionName = old.databaseDefinitionName;
            this.databaseInstanceName = old.databaseInstanceName;
        }
    }
}
