/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Database Definition</b> entity
    /// </summary>
    public partial class DatabaseDefinition : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private string schemaSourceDatabaseName;
        private DatabaseLayoutType layoutType;
        private string databaseInstanceNamePattern;
        private string databaseNamePattern;
        private int sliceCount;
        private int partitionCount;
        private PartitionRangeType partitionRangeType;
        private string partitionFunction;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the name of the database containing the <b>Schema Template</b>.
        /// </summary>
        [DBColumn(Size = 128)]
        public string SchemaSourceDatabaseName
        {
            get { return schemaSourceDatabaseName; }
            set { schemaSourceDatabaseName = value; }
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
        public int SliceCount
        {
            get { return sliceCount; }
            set { sliceCount = value; }
        }

        /// <summary>
        /// Gets or sets the default number of partitions per physical database instances.
        /// </summary>
        [DBColumn]
        public int PartitionCount
        {
            get { return partitionCount; }
            set { partitionCount = value; }
        }

        /// <summary>
        /// Gets or sets the range type for the partition function
        /// </summary>
        [DBColumn]
        public PartitionRangeType PartitionRangeType
        {
            get { return partitionRangeType; }
            set { partitionRangeType = value; }
        }

        [DBColumn(Size = 50)]
        public string PartitionFunction
        {
            get { return partitionFunction; }
            set { partitionFunction = value; }
        }

        #endregion
        #region Navigation Properties

        public Cluster Cluster
        {
            get { return (Cluster)ParentReference.Value; }
        }

        /// <summary>
        /// Gets the <b>Federation</b> object to which this <b>Database Definition</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        public Federation Federation
        {
            get { return (Federation)ParentReference.Value; }
        }

        public Dictionary<string, DatabaseVersion> DatabaseVersions
        {
            get { return GetChildren<DatabaseVersion>(); }
            set { SetChildren<DatabaseVersion>(value); }
        }

        public Dictionary<string, FileGroup> FileGroups
        {
            get { return GetChildren<FileGroup>(); }
            set { SetChildren<FileGroup>(value); }
        }

        public Dictionary<string, Slice> Slices
        {
            get { return GetChildren<Slice>(); }
            set { SetChildren<Slice>(value); }
        }

        public Dictionary<string, DeploymentPackage> DeploymentPackages
        {
            get { return GetChildren<DeploymentPackage>(); }
            set { SetChildren<DeploymentPackage>(value); }
        }

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
        public DatabaseDefinition(Context context, Cluster parent)
            : base(context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DatabaseDefinition(Context context, Federation parent)
            : base(context, parent)
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
            base.EntityType = EntityType.DatabaseDefinition;
            base.EntityGroup = EntityGroup.Federation | EntityGroup.Layout;

            this.schemaSourceDatabaseName = string.Empty;
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
            this.schemaSourceDatabaseName = old.schemaSourceDatabaseName;
            this.layoutType = old.layoutType;
            this.databaseInstanceNamePattern = old.databaseInstanceNamePattern;
            this.databaseNamePattern = old.databaseNamePattern;
            this.sliceCount = old.sliceCount;
            this.partitionCount = old.partitionCount;
            this.partitionRangeType = old.partitionRangeType;
            this.partitionFunction = old.partitionFunction;
        }

        protected override Type[] CreateChildTypes()
        {
            return new Type[] {
                typeof(DatabaseVersion),
                typeof(FileGroup),
                typeof(Slice),
                typeof(DeploymentPackage),
                typeof(DatabaseInstance),
            };
        }

        #endregion
        #region Layout Mapping Functions

        public List<Slice> GenerateSlices(string[] sliceNames, long[][] sliceLimits, string[][] partitionNames, long[][][] partitionLimits)
        {
            List<Slice> slices = new List<Slice>();

            for (int si = 0; si < sliceNames.Length; si++)
            {
                Slice ns = new Slice(Context, this);

                ns.Name = sliceNames[si];
                ns.From = sliceLimits[si][0];
                ns.To = sliceLimits[si][1];
                ns.Save();

                ns.GeneratePartitions(partitionNames[si], partitionLimits[si]);

                slices.Add(ns);
            }

            return slices;
        }


        public DatabaseInstance GenerateDatabaseInstance(ServerInstance serverInstance, Slice slice, DatabaseVersion databaseVersion)
        {
            return GenerateDatabaseInstance(serverInstance, slice, databaseVersion, databaseInstanceNamePattern, databaseNamePattern, databaseVersion.SizeMultiplier, true);
        }

        public DatabaseInstance GenerateDatabaseInstance(ServerInstance serverInstance, Slice slice, DatabaseVersion databaseVersion, string namePattern, string databaseNamePattern, double sizeFactor, bool generateFileGroups)
        {
            return GenerateDatabaseInstance(serverInstance, null, null, slice, databaseVersion, namePattern, databaseNamePattern, sizeFactor, generateFileGroups);
        }

        public DatabaseInstance GenerateDatabaseInstance(ServerInstance serverInstance, List<DiskVolume> dataDiskVolumes, List<DiskVolume> logDiskVolumes, Slice slice, DatabaseVersion databaseVersion, string namePattern, string databaseNamePattern, double sizeFactor, bool generateFileGroups)
        {
            // --- Create the new database instance and set name
            DatabaseInstance ndi = new DatabaseInstance(Context, this);

            ndi.ServerInstanceReference.Guid = serverInstance.Guid;
            ndi.SliceReference.Guid = slice.Guid;
            ndi.DatabaseVersionReference.Guid = databaseVersion.Guid;

            ndi.Name = Util.ResolveExpression(ndi, namePattern);
            ndi.DatabaseName = Util.ResolveExpression(ndi, databaseNamePattern);

            ndi.Save();

            if (generateFileGroups)
            {
                ndi.ServerInstance.Machine.LoadDiskVolumes(false);

                this.LoadFileGroups(false);

                slice.LoadPartitions(false);
                List<Partition> partitions = new List<Partition>(slice.Partitions.Values.OrderBy(i => i.Number));
                List<FileGroup> filegroups = new List<FileGroup>(this.FileGroups.Values.OrderBy(i => i.Number));

                for (int fi = 0; fi < filegroups.Count; fi++)
                {
                    // --- Create data and "log" file groups ---
                    if (filegroups[fi].LayoutType == FileGroupLayoutType.Monolithic ||
                        filegroups[fi].FileGroupType == FileGroupType.Log)
                    {
                        DatabaseInstanceFileGroup nfg = new DatabaseInstanceFileGroup(Context, ndi);
                        nfg.FileGroupType = filegroups[fi].FileGroupType;
                        nfg.FileGroupName = nfg.Name = filegroups[fi].FileGroupName;
                        nfg.FileGroupReference.Guid = filegroups[fi].Guid;
                        nfg.PartitionReference.Guid = Guid.Empty;
                        nfg.AllocatedSpace = (long)(filegroups[fi].AllocatedSpace * sizeFactor);
                        nfg.Save();

                        nfg.GenerateInstanceFiles(dataDiskVolumes, sizeFactor);
                    }
                    else if (filegroups[fi].LayoutType == FileGroupLayoutType.Sliced)
                    {
                        for (int pi = 0; pi < partitions.Count; pi++)
                        {
                            DatabaseInstanceFileGroup nfg = new DatabaseInstanceFileGroup(Context, ndi);
                            nfg.FileGroupType = filegroups[fi].FileGroupType;
                            nfg.FileGroupName = nfg.Name = string.Format("{0}_{1}", filegroups[fi].FileGroupName, pi);
                            nfg.FileGroupReference.Guid = filegroups[fi].Guid;
                            nfg.PartitionReference.Guid = partitions[pi].Guid;
                            nfg.AllocatedSpace = (long)(filegroups[fi].AllocatedSpace * sizeFactor);
                            nfg.Save();

                            nfg.GenerateInstanceFiles(dataDiskVolumes, sizeFactor);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            return ndi;
        }

        public List<DatabaseInstance> GenerateDatabaseInstances(ServerInstance[][] serverInstances)
        {
            return GenerateDatabaseInstances(serverInstances, databaseInstanceNamePattern, databaseInstanceNamePattern, 1.0, true);
        }

        public List<DatabaseInstance> GenerateDatabaseInstances(ServerInstance[][] serverInstances, string namePattern, string databaseNamePattern, double sizeFactor, bool generateFileGroups)
        {
            List<DatabaseInstance> instances = new List<DatabaseInstance>();

            LoadDatabaseVersions(false);
            LoadSlices(false);

            List<Slice> slices = new List<Slice>(this.Slices.Values.OrderBy(i => i.Number));

            for (int si = 0; si < slices.Count; si++)
            {
                Slice slice = slices[si];
                // **** TODO review this part and add [$Number] to pattern if mirrored
                // to avoid name collision under databaseinstance
                foreach (DatabaseVersion rs in this.DatabaseVersions.Values)
                {
                    DatabaseInstance ndi = GenerateDatabaseInstance(serverInstances[si][rs.Number], slices[si], rs, namePattern, databaseNamePattern, sizeFactor, generateFileGroups);

                    instances.Add(ndi);
                }
            }

            return instances;
        }

        #endregion

        public void GenerateDefaultChildren(Guid serverVersionGuid)
        {
            // If not sliced, then create a default slice of FULL
            if (layoutType == DatabaseLayoutType.Mirrored ||
                layoutType == DatabaseLayoutType.Monolithic)
            {
                Slice sl = new Slice(Context, this);
                sl.Name = Constants.FullSliceName;
                sl.Save();
            }

            // Add primary filegroup
            FileGroup fg = new FileGroup(Context, this);
            fg.Name = Constants.PrimaryFileGroupName;
            fg.FileGroupType = FileGroupType.Data;
            switch (layoutType)
            {
                case DatabaseLayoutType.Sliced:
                    fg.LayoutType = FileGroupLayoutType.Sliced;
                    break;
                case DatabaseLayoutType.Monolithic:
                case DatabaseLayoutType.Mirrored:
                    fg.LayoutType = FileGroupLayoutType.Monolithic;
                    break;
                default:
                    throw new NotImplementedException();
            }
            fg.AllocationType = FileGroupAllocationType.CrossVolume;
            fg.DiskVolumeType = DiskVolumeType.Data;
            fg.FileGroupName = Constants.PrimaryFileGroupName;
            fg.AllocatedSpace = 0x8000000;      // 128 MB
            fg.FileCount = 0;
            fg.Save();

            // Add "log" file group
            fg = new FileGroup(Context, this);
            fg.Name = Constants.LogFileGroupName;
            fg.FileGroupType = FileGroupType.Log;
            fg.LayoutType = FileGroupLayoutType.Monolithic;
            fg.AllocationType = FileGroupAllocationType.CrossVolume;
            fg.DiskVolumeType = DiskVolumeType.Log;
            fg.FileGroupName = Constants.LogFileGroupName;
            fg.AllocatedSpace = 0x1000000;      // 16 MB
            fg.FileCount = 0;
            fg.Save();

            // Create default "HOT" version
            DatabaseVersion dv = new DatabaseVersion(Context, this);
            dv.Name = "HOT";
            dv.ServerVersionReference.Guid = serverVersionGuid;
            dv.SizeMultiplier = 1.0f;
            dv.Save();
        }


        //

        public DatabaseInstance GetDatabaseInstance(DatabaseVersion databaseVersion, long partitionKeyValue)
        {
            // *** TODO: partitioning key interval limits (inclusive, exclusive!)
            LoadSlices(false);
            Slice slice = this.Slices.Values.First(x => x.From <= partitionKeyValue && x.To >= partitionKeyValue);

            this.LoadDatabaseInstances(false);

            return this.DatabaseInstances.Values.First(x => x.DatabaseVersionReference.Guid == databaseVersion.Guid && x.Slice.Guid == slice.Guid);
        }

        /// <summary>
        /// Returns a SqlConnectionStringBuilder initialized to point to
        /// the database template of this database definition.
        /// </summary>
        /// <returns>The connection string builder object.</returns>
        public SqlConnectionStringBuilder GetConnectionString()
        {
            SqlConnectionStringBuilder csb = this.Federation.SchemaSourceServerInstance.GetConnectionString();

            csb.InitialCatalog = this.schemaSourceDatabaseName;

            return csb;
        }
    }
}
