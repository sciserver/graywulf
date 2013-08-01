/* Copyright */
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Federation</b> entity
    /// </summary>
    public partial class Federation : Entity
    {
        public static class AppSettings
        {
            private static string GetValue(string key)
            {
                return (string)((NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Registry/Federation"))[key];
            }

            public static string FederationName
            {
                get { return GetValue("FederationName"); }
            }
        }

        public enum ReferenceType : int
        {
            ControllerMachine = 1,
            SchemaSourceServerInstance = 2,
            MyDBDatabaseVersion = 3,
            TempDatabaseVersion = 4,
            CodeDatabaseVersion = 5,
        }

        #region Member Variables

        // --- Background storage for properties ---
        private string shortTitle;
        private string longTitle;
        private string email;
        private string queryFactoryTypeName;

        #endregion
        #region Member Access Properties

        [DBColumn(Size = 50)]
        public string ShortTitle
        {
            get { return shortTitle; }
            set { shortTitle = value; }
        }

        [DBColumn(Size = 256)]
        public string LongTitle
        {
            get { return longTitle; }
            set { longTitle = value; }
        }

        [DBColumn(Size = 128)]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [DBColumn(Size = 1024)]
        public string QueryFactoryTypeName
        {
            get { return queryFactoryTypeName; }
            set { queryFactoryTypeName = value; }
        }

        [XmlIgnore]
        public DatabaseVersion MyDBDatabaseVersion
        {
            get { return MyDBDatabaseVersionReference.Value; }
            set { MyDBDatabaseVersionReference.Value = value; }
        }

        [XmlIgnore]
        public DatabaseVersion TempDatabaseVersion
        {
            get { return TempDatabaseVersionReference.Value; }
            set { TempDatabaseVersionReference.Value = value; }
        }

        [XmlIgnore]
        public DatabaseVersion CodeDatabaseVersion
        {
            get { return CodeDatabaseVersionReference.Value; }
            set { CodeDatabaseVersionReference.Value = value; }
        }

        /// <summary>
        /// Gets the controller machine of this federation.
        /// </summary>
        [XmlIgnore]
        public Machine ControllerMachine
        {
            get { return ControllerMachineReference.Value; }
            set { ControllerMachineReference.Value = value; }
        }

        /// <summary>
        /// Gets the server instance containing the template databases.
        /// </summary>
        [XmlIgnore]
        public ServerInstance SchemaSourceServerInstance
        {
            get { return SchemaSourceServerInstanceReference.Value; }
            set { SchemaSourceServerInstanceReference.Value = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Domain</b> object to which this <b>Federation</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public Domain Domain
        {
            get { return (Domain)ParentReference.Value; }
        }

        [XmlIgnore]
        public EntityReference<DatabaseVersion> MyDBDatabaseVersionReference
        {
            get { return (EntityReference<DatabaseVersion>)EntityReferences[(int)ReferenceType.MyDBDatabaseVersion]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("MyDBDatabaseVersion")]
        public string MyDBDatabaseVersion_ForXml
        {
            get { return MyDBDatabaseVersionReference.Name; }
            set { MyDBDatabaseVersionReference.Name = value; }
        }

        [XmlIgnore]
        public EntityReference<DatabaseVersion> TempDatabaseVersionReference
        {
            get { return (EntityReference<DatabaseVersion>)EntityReferences[(int)ReferenceType.TempDatabaseVersion]; }
        }

        [XmlElement("TempDatabaseVersion")]
        public string TempDatabaseVersion_ForXml
        {
            get { return TempDatabaseVersionReference.Name; }
            set { TempDatabaseVersionReference.Name = value; }
        }

        [XmlIgnore]
        public EntityReference<DatabaseVersion> CodeDatabaseVersionReference
        {
            get { return (EntityReference<DatabaseVersion>)EntityReferences[(int)ReferenceType.CodeDatabaseVersion]; }
        }

        [XmlElement("CodeDatabaseVersion")]
        public string CodeDatabaseVersion_ForXml
        {
            get { return CodeDatabaseVersionReference.Name; }
            set { CodeDatabaseVersionReference.Name = value; }
        }

        /// <summary>
        /// Gets the reference object to the controller machine of this federation.
        /// </summary>
        [XmlIgnore]
        public EntityReference<Machine> ControllerMachineReference
        {
            get { return (EntityReference<Machine>)EntityReferences[(int)ReferenceType.ControllerMachine]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("ControllerMachine")]
        public string ControllerMachine_ForXml
        {
            get { return ControllerMachineReference.Name; }
            set { ControllerMachineReference.Name = value; }
        }

        /// <summary>
        /// Gets the reference object to the schema source server instance (containing the
        /// database templates).
        /// </summary>
        [XmlIgnore]
        public EntityReference<ServerInstance> SchemaSourceServerInstanceReference
        {
            get { return (EntityReference<ServerInstance>)EntityReferences[(int)ReferenceType.SchemaSourceServerInstance]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("SchemaSourceServerInstance")]
        public string SchemaSourceServerInstance_ForXml
        {
            get { return SchemaSourceServerInstanceReference.Name; }
            set { SchemaSourceServerInstanceReference.Name = value; }
        }

        public Dictionary<string, DatabaseDefinition> DatabaseDefinitions
        {
            get { return GetChildren<DatabaseDefinition>(); }
            set { SetChildren<DatabaseDefinition>(value); }
        }

        public Dictionary<string, RemoteDatabase> RemoteDatabases
        {
            get { return GetChildren<RemoteDatabase>(); }
            set { SetChildren<RemoteDatabase>(value); }
        }

        public Dictionary<string, QueueDefinition> QueueDefinitions
        {
            get { return GetChildren<QueueDefinition>(); }
            set { SetChildren<QueueDefinition>(value); }
        }

        public Dictionary<string, JobDefinition> JobDefinitions
        {
            get { return GetChildren<JobDefinition>(); }
            set { SetChildren<JobDefinition>(value); }
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
        public Federation()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Federation</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public Federation(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public Federation(Context context, Cluster parent)
            : base(context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Federation</b> objects.
        /// </summary>
        /// <param name="old">The <b>Federation</b> to copy from.</param>
        public Federation(Federation old)
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
            base.EntityType = EntityType.Federation;
            base.EntityGroup = EntityGroup.Federation | EntityGroup.Layout | EntityGroup.Log | EntityGroup.Jobs | EntityGroup.Security;

            this.shortTitle = String.Empty;
            this.longTitle = String.Empty;
            this.email = String.Empty;
            this.queryFactoryTypeName = typeof(Jobs.Query.SqlQueryFactory).AssemblyQualifiedName;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Federation</b> object to create the deep copy from.</param>
        private void CopyMembers(Federation old)
        {
            this.shortTitle = old.shortTitle;
            this.longTitle = old.longTitle;
            this.email = old.email;
            this.queryFactoryTypeName = old.queryFactoryTypeName;
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<DatabaseVersion>((int)ReferenceType.MyDBDatabaseVersion),
                new EntityReference<DatabaseVersion>((int)ReferenceType.TempDatabaseVersion),
                new EntityReference<DatabaseVersion>((int)ReferenceType.CodeDatabaseVersion),
                new EntityReference<Machine>((int)ReferenceType.ControllerMachine),
                new EntityReference<ServerInstance>((int)ReferenceType.SchemaSourceServerInstance),
            };
        }

        protected override Type[] CreateChildTypes()
        {
            return new Type[] 
            {
                typeof(DatabaseDefinition),
                typeof(RemoteDatabase),
                typeof(QueueDefinition),
                typeof(JobDefinition),
            };
        }

        #endregion

        public void GenerateDefaultChildren(Guid myDbServerVersionGuid)
        {
            DatabaseDefinition dd = new DatabaseDefinition(Context, this);
            dd.Name = Constants.MyDbName;
            dd.LayoutType = DatabaseLayoutType.Monolithic;
            dd.DatabaseInstanceNamePattern = String.Format(Constants.MyDbInstanceNamePattern, Constants.MyDbName);
            dd.DatabaseNamePattern = String.Format(Constants.MyDbNamePattern, this.Name, Constants.MyDbName);
            dd.SliceCount = 1;
            dd.PartitionCount = 1;
            dd.Save();

            FileGroup fg = new FileGroup(Context, dd);
            fg.Name = Constants.PrimaryFileGroupName;
            fg.LayoutType = FileGroupLayoutType.Monolithic;
            fg.FileGroupType = FileGroupType.Data;
            fg.DiskVolumeType = DiskVolumeType.Data;
            fg.FileGroupName = Constants.PrimaryFileGroupName;
            fg.AllocatedSpace = 0x8000000;  // 128 MB
            fg.FileCount = 0;
            fg.Save();

            fg = new FileGroup(Context, dd);
            fg.Name = Constants.LogFileGroupName;
            fg.LayoutType = FileGroupLayoutType.Monolithic;
            fg.FileGroupType = FileGroupType.Log;
            fg.DiskVolumeType = DiskVolumeType.Log;
            fg.FileGroupName = Constants.LogFileGroupName;
            fg.AllocatedSpace = 0x1000000;  // 16 MB
            fg.FileCount = 0;
            fg.Save();

            Slice sl = new Slice(Context, dd);
            sl.Name = Constants.FullSliceName;
            sl.Save();

            DatabaseVersion dv = new DatabaseVersion(Context, dd);
            dv.Name = Constants.MyDbName;
            dv.ServerVersionReference.Guid = myDbServerVersionGuid;
            dv.Save();
            this.MyDBDatabaseVersion = dv;

            // Job definitions
            var jd = new JobDefinition(Context, this);
            jd.Name = typeof(Jobs.ExportTable.ExportTableJob).Name;
            jd.WorkflowTypeName = typeof(Jobs.ExportTable.ExportTableJob).AssemblyQualifiedName;
            jd.Save();

            jd = new JobDefinition(Context, this);
            jd.Name = typeof(Jobs.ExportTable.ExportMaintenanceJob).Name;
            jd.WorkflowTypeName = typeof(Jobs.ExportTable.ExportMaintenanceJob).AssemblyQualifiedName;
            jd.Save();

            jd = new JobDefinition(Context, this);
            jd.Name = typeof(Jobs.Query.SqlQueryJob).Name;
            jd.WorkflowTypeName = typeof(Jobs.Query.SqlQueryJob).AssemblyQualifiedName;
            jd.Settings = Util.SaveSettings(new Dictionary<string, string>()
            {
                // TODO: update these in the query factory
                {"HotDatabaseVersionName", "HOT"},
                {"StatDatabaseVersionName", "STAT"},
                {"DefaultSchemaName", "dbo"},
                {"DefaultDatasetName", Constants.MyDbName},
                {"DefaultTableName", "outputtable"},
                {"TemporarySchemaName", "dbo"},
                {"LongQueryTimeout", "1200"},
            });
            jd.Save();

            this.Save();
        }

    }
}
