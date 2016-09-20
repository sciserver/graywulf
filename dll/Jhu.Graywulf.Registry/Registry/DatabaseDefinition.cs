/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.ComponentModel;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Database Definition</b> entity
    /// </summary>
    public partial class DatabaseDefinition : Entity
    {
        enum ReferenceType : int
        {
        }

        #region Member Variables

        // --- Background storage for properties ---
        private DatabaseLayoutType layoutType;
        private string databaseInstanceNamePattern;
        private string databaseNamePattern;
        private int sliceCount;
        private int partitionCount;
        private PartitionRangeType partitionRangeType;
        private string partitionFunction;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.DatabaseDefinition; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Federation; }
        }

        /// <summary>
        /// Gets or sets the value indicating the physical layout of the database.
        /// </summary>
        [DBColumn]
        public DatabaseLayoutType LayoutType
        {
            get { return layoutType; }
            set { layoutType = value; }
        }

        [DBColumn(Size = 256)]
        public string DatabaseInstanceNamePattern
        {
            get { return databaseInstanceNamePattern; }
            set { databaseInstanceNamePattern = value; }
        }

        [DBColumn(Size = 256)]
        public string DatabaseNamePattern
        {
            get { return databaseNamePattern; }
            set { databaseNamePattern = value; }
        }

        /// <summary>
        /// Gets or sets the number of slices used for the database layout.
        /// </summary>
        [DBColumn]
        [DefaultValue(1)]
        public int SliceCount
        {
            get { return sliceCount; }
            set { sliceCount = value; }
        }

        /// <summary>
        /// Gets or sets the default number of partitions per physical database instances.
        /// </summary>
        [DBColumn]
        [DefaultValue(1)]
        public int PartitionCount
        {
            get { return partitionCount; }
            set { partitionCount = value; }
        }

        /// <summary>
        /// Gets or sets the range type for the partition function
        /// </summary>
        [DBColumn]
        [DefaultValue(PartitionRangeType.Left)]
        public PartitionRangeType PartitionRangeType
        {
            get { return partitionRangeType; }
            set { partitionRangeType = value; }
        }

        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string PartitionFunction
        {
            get { return partitionFunction; }
            set { partitionFunction = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Federation</b> object to which this <b>Database Definition</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public Federation Federation
        {
            get { return (Federation)ParentReference.Value; }
        }

        [XmlIgnore]
        public Dictionary<string, DatabaseVersion> DatabaseVersions
        {
            get { return GetChildren<DatabaseVersion>(); }
            set { SetChildren<DatabaseVersion>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, FileGroup> FileGroups
        {
            get { return GetChildren<FileGroup>(); }
            set { SetChildren<FileGroup>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, Slice> Slices
        {
            get { return GetChildren<Slice>(); }
            set { SetChildren<Slice>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, DeploymentPackage> DeploymentPackages
        {
            get { return GetChildren<DeploymentPackage>(); }
            set { SetChildren<DeploymentPackage>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, DatabaseInstance> DatabaseInstances
        {
            get { return GetChildren<DatabaseInstance>(); }
            set { SetChildren<DatabaseInstance>(value); }
        }

        #endregion
        #region Validation Properties
        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public DatabaseDefinition()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Database Definition</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public DatabaseDefinition(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DatabaseDefinition(Federation parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Database Definition</b> objects.
        /// </summary>
        /// <param name="old">The <b>Database Definition</b> to copy from.</param>
        public DatabaseDefinition(DatabaseDefinition old)
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
            this.layoutType = DatabaseLayoutType.Monolithic;
            this.databaseInstanceNamePattern = Constants.MonolithicDatabaseInstanceNamePattern;
            this.databaseNamePattern = Constants.MonolithicDatabaseNamePattern;
            this.sliceCount = 0;
            this.partitionCount = 0;
            this.partitionRangeType = 0;
            this.partitionFunction = string.Empty;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Database Definition</b> object to create the deep copy from.</param>
        private void CopyMembers(DatabaseDefinition old)
        {
            this.layoutType = old.layoutType;
            this.databaseInstanceNamePattern = old.databaseInstanceNamePattern;
            this.databaseNamePattern = old.databaseNamePattern;
            this.sliceCount = old.sliceCount;
            this.partitionCount = old.partitionCount;
            this.partitionRangeType = old.partitionRangeType;
            this.partitionFunction = old.partitionFunction;
        }

        internal override bool CompareMembers(Entity other)
        {
            bool eq = base.CompareMembers(other);
            var o = other as DatabaseDefinition;

            eq &= this.layoutType == o.layoutType;
            eq &= this.databaseInstanceNamePattern == o.databaseInstanceNamePattern;
            eq &= this.databaseNamePattern == o.databaseNamePattern;
            eq &= this.sliceCount == o.sliceCount;
            eq &= this.partitionCount == o.partitionCount;
            eq &= this.partitionRangeType == o.partitionRangeType;
            eq &= this.partitionFunction == o.partitionFunction;

            return eq;
        }

        internal override void UpdateMembers(Entity other)
        {
            base.UpdateMembers(other);
            var o = other as DatabaseDefinition;
            CopyMembers(o);
        }

        public override object Clone()
        {
            return new DatabaseDefinition(this);
        }

        protected override EntityType[] CreateChildTypes()
        {
            return new EntityType[] {
                EntityType.DatabaseVersion,
                EntityType.FileGroup,
                EntityType.Slice,
                EntityType.DeploymentPackage,
                EntityType.DatabaseInstance,
            };
        }

        #endregion

        private DatabaseInstance GetSchemaDatabaseInstance()
        {
            // TODO: make it a stored procedure
            DatabaseVersion dv = null;

            LoadDatabaseVersions(false);
            LoadDatabaseInstances(false);

            if (DatabaseVersions.ContainsKey(Constants.SchemaDatabaseVersionName))
            {
                dv = DatabaseVersions[Constants.SchemaDatabaseVersionName];
            }
            else if (DatabaseVersions.ContainsKey(Constants.ProdDatabaseVersionName))
            {
                dv = DatabaseVersions[Constants.ProdDatabaseVersionName];
            }
            else
            {
                dv = DatabaseVersions.Values.FirstOrDefault();
            }

            if (dv != null)
            {

                var di = DatabaseInstances.Values.Where(i =>
                    i.ServerInstance.Machine.DeploymentState == DeploymentState.Deployed &&
                    i.ServerInstance.Machine.RunningState == RunningState.Running &&
                    i.DeploymentState == DeploymentState.Deployed &&
                    i.RunningState == RunningState.Attached &&
                    i.DatabaseVersionReference.Guid == dv.Guid).ToArray();

                if (di.Length > 0)
                {
                    return di[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a SqlConnectionStringBuilder initialized to point to
        /// the database template of this database definition.
        /// </summary>
        /// <returns>The connection string builder object.</returns>
        public SqlConnectionStringBuilder GetSchemaConnectionString()
        {
            var di = GetSchemaDatabaseInstance();
            return di.GetConnectionString();
        }
    }
}
