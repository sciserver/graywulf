using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class GraywulfDataset : Schema.SqlServer.SqlServerDataset, IRegistryContextObject, ICloneable
    {
        #region Private member variables

        [NonSerialized]
        private RegistryContext context;

        [NonSerialized]
        private EntityReference<DatabaseDefinition> databaseDefinitionReference;

        [NonSerialized]
        private EntityReference<DatabaseVersion> databaseVersionReference;

        [NonSerialized]
        private EntityReference<DatabaseInstance> databaseInstanceReference;

        #endregion
        #region Properties

        public RegistryContext RegistryContext
        {
            get { return context; }
            set { context = value; }
        }

        [DataMember]
        public EntityReference<DatabaseDefinition> DatabaseDefinitionReference
        {
            get { return databaseDefinitionReference; }
            set { databaseDefinitionReference = value; }
        }

        [DataMember]
        public EntityReference<DatabaseVersion> DatabaseVersionReference
        {
            get { return databaseVersionReference; }
            set { databaseVersionReference = value; }
        }

        [DataMember]
        public EntityReference<DatabaseInstance> DatabaseInstanceReference
        {
            get { return databaseInstanceReference; }
            set { databaseInstanceReference = value; }
        }

        [IgnoreDataMember]
        public override string ConnectionString
        {
            get
            {
                if (base.ConnectionString == null)
                {
                    CacheSchemaConnectionString();
                }

                return base.ConnectionString;
            }
            set
            {
                base.ConnectionString = value;
            }
        }

        [IgnoreDataMember]
        public bool IsSpecificInstanceRequired
        {
            get { return !databaseInstanceReference.IsEmpty; }
        }

        #endregion
        #region Constructors and initializers

        public GraywulfDataset(RegistryContext context)
            : base()
        {
            InitializeMembers(new StreamingContext());

            this.context = context;
        }

        public GraywulfDataset(RegistryContext context, DatasetBase old)
            : base(old)
        {
            InitializeMembers(new StreamingContext());

            this.context = context;
        }

        public GraywulfDataset(GraywulfDataset old)
            : base(old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.context = null;

            this.databaseDefinitionReference = new EntityReference<DatabaseDefinition>(this);
            this.databaseVersionReference = new EntityReference<DatabaseVersion>(this);
            this.databaseInstanceReference = new EntityReference<DatabaseInstance>(this);
        }

        [OnDeserialized]
        private void UpdateMembers(StreamingContext context)
        {
            this.databaseDefinitionReference.ReferencingObject = this;
            this.databaseVersionReference.ReferencingObject = this;
            this.databaseInstanceReference.ReferencingObject = this;
        }

        private void CopyMembers(GraywulfDataset old)
        {
            this.context = old.context;

            this.databaseDefinitionReference = new EntityReference<DatabaseDefinition>(this, old.databaseDefinitionReference);
            this.databaseVersionReference = new EntityReference<DatabaseVersion>(this, old.databaseVersionReference);
            this.databaseInstanceReference = new EntityReference<DatabaseInstance>(this, old.databaseInstanceReference);
        }

        #endregion

        public void CacheSchemaConnectionString()
        {
            if (!databaseInstanceReference.IsEmpty)
            {
                base.ConnectionString = databaseInstanceReference.Value.GetConnectionString().ConnectionString;
            }
            else if (!databaseVersionReference.IsEmpty)
            {
                base.ConnectionString = databaseVersionReference.Value.DatabaseDefinition.GetSchemaConnectionString().ConnectionString;
            }
            else
            {
                base.ConnectionString = databaseDefinitionReference.Value.GetSchemaConnectionString().ConnectionString;
            }
        }
    }
}
