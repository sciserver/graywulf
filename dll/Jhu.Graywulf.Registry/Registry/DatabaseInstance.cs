/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Data.SqlClient;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Database Instance</b> entity
    /// </summary>
    public partial class DatabaseInstance : Entity
    {
        enum ReferenceType : int
        {
            ServerInstance = 1,
            Slice = 2,
            DatabaseVersion = 3,
        }

        #region Member Variables

        // --- Background storage for properties ---
        private string databaseName;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the name of the referenced physical database.
        /// </summary>
        [DBColumn(Size = 128)]
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Database Definition</b> object to which this <b>Database Instance</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        public DatabaseDefinition DatabaseDefinition
        {
            get { return (DatabaseDefinition)ParentReference.Value; }
        }

        /// <summary>
        /// Gets the reference object to the server instance containing this database instance.
        /// </summary>
        [XmlIgnore]
        public EntityReference<ServerInstance> ServerInstanceReference
        {
            get { return (EntityReference<ServerInstance>)EntityReferences[(int)ReferenceType.ServerInstance]; }
        }

        /// <summary>
        /// Gets the server instance containing this database.
        /// </summary>
        [XmlIgnore]
        public ServerInstance ServerInstance
        {
            get { return ServerInstanceReference.Value; }
            set { ServerInstanceReference.Value = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("ServerInstance")]
        public string ServerInstance_ForXml
        {
            get { return ServerInstanceReference.Name; }
            set { ServerInstanceReference.Name = value; }
        }

        /// <summary>
        /// Gets the reference object to the slice represented by this
        /// database instance.
        /// </summary>
        [XmlIgnore]
        public EntityReference<Slice> SliceReference
        {
            get { return (EntityReference<Slice>)EntityReferences[(int)ReferenceType.Slice]; }
        }

        /// <summary>
        /// Gets the slice object represented by the database instance.
        /// </summary>
        [XmlIgnore]
        public Slice Slice
        {
            get { return SliceReference.Value; }
            set { SliceReference.Value = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("Slice")]
        public string Slice_ForXml
        {
            get { return SliceReference.Name; }
            set { SliceReference.Name = value; }
        }

        /// <summary>
        /// Gets the reference object to the database version represented by
        /// this database instance.
        /// </summary>
        [XmlIgnore]
        public EntityReference<DatabaseVersion> DatabaseVersionReference
        {
            get { return (EntityReference<DatabaseVersion>)EntityReferences[(int)ReferenceType.DatabaseVersion]; }
        }

        /// <summary>
        /// Gets the database version represented by this database instance.
        /// </summary>
        [XmlIgnore]
        public DatabaseVersion DatabaseVersion
        {
            get { return DatabaseVersionReference.Value; }
            set { DatabaseVersionReference.Value = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("DatabaseVersion")]
        public string DatabaseVersion_ForXml
        {
            get { return DatabaseVersionReference.Name; }
            set { DatabaseVersionReference.Name = value; }
        }

        [XmlIgnore]
        public Dictionary<string, DatabaseInstanceFileGroup> FileGroups
        {
            get { return GetChildren<DatabaseInstanceFileGroup>(); }
            set { SetChildren<DatabaseInstanceFileGroup>(value); }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public DatabaseInstance()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Database Instance</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public DatabaseInstance(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DatabaseInstance(DatabaseDefinition parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Database Instance</b> objects.
        /// </summary>
        /// <param name="old">The <b>Database Instance</b> to copy from.</param>
        public DatabaseInstance(DatabaseInstance old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their initial values.
        /// </summary>
        /// <remarks>
        /// This function is called by the contructors.
        /// </remarks>
        private void InitializeMembers()
        {
            base.EntityType = EntityType.DatabaseInstance;
            base.EntityGroup = EntityGroup.Layout;

            this.databaseName = string.Empty;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Database Instance</b> object to create the deep copy from.</param>
        private void CopyMembers(DatabaseInstance old)
        {
            this.databaseName = old.databaseName;
        }

        public override object Clone()
        {
            return new DatabaseInstance(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<ServerInstance>((int)ReferenceType.ServerInstance),
                new EntityReference<Slice>((int)ReferenceType.Slice),
                new EntityReference<DatabaseVersion>((int)ReferenceType.DatabaseVersion),
            };
        }

        protected override Type[] CreateChildTypes()
        {
            return new Type[] {
                typeof(DatabaseInstanceFileGroup)
            };
        }

        #endregion

        /// <summary>
        /// Returns a SqlConnectionStringBuilder initialized to a connection string
        /// pointin to the database represented by this object.
        /// </summary>
        /// <returns>The connection string builder object.</returns>
        public SqlConnectionStringBuilder GetConnectionString()
        {
            SqlConnectionStringBuilder csb = this.ServerInstance.GetConnectionString();

            csb.InitialCatalog = this.databaseName;

            return csb;
        }

        public Schema.SqlServer.SqlServerDataset GetDataset()
        {
            SqlConnectionStringBuilder csb = this.ServerInstance.GetConnectionString();
            csb.InitialCatalog = this.databaseName;

            return new Schema.SqlServer.SqlServerDataset()
            {
                ConnectionString = csb.ConnectionString
            };
        }
    }
}
