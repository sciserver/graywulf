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
        enum ReferenceType : int
        {
            ControllerMachine = 1,
            SchemaSourceServerInstance = 2,
            MyDBDatabaseVersion = 3,
            TempDatabaseVersion = 4,
            CodeDatabaseVersion = 5,
        }

        #region Member Variables

        // --- Background storage for properties ---
        private string queryFactory;
        private string fileFormatFactory;
        private string streamFactory;
        private string shortTitle;
        private string longTitle;
        private string email;
        private string copyright;
        private string disclaimer;

        #endregion
        #region Member Access Properties

        [DBColumn(Size = 1024)]
        public string QueryFactory
        {
            get { return queryFactory; }
            set { queryFactory = value; }
        }

        [DBColumn(Size = 1024)]
        public string FileFormatFactory
        {
            get { return fileFormatFactory; }
            set { fileFormatFactory = value; }
        }

        [DBColumn(Size = 1024)]
        public string StreamFactory
        {
            get { return streamFactory; }
            set { streamFactory = value; }
        }

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
        public string Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }

        [DBColumn(Size = 1024)]
        public string Disclaimer
        {
            get { return disclaimer; }
            set { disclaimer = value; }
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

        [XmlIgnore]
        public Dictionary<string, DatabaseDefinition> DatabaseDefinitions
        {
            get { return GetChildren<DatabaseDefinition>(); }
            set { SetChildren<DatabaseDefinition>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, RemoteDatabase> RemoteDatabases
        {
            get { return GetChildren<RemoteDatabase>(); }
            set { SetChildren<RemoteDatabase>(value); }
        }

        [XmlIgnore]
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
        public Federation(Domain parent)
            : base(parent.Context, parent)
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

            this.queryFactory = String.Empty;
            this.fileFormatFactory = String.Empty;
            this.streamFactory = String.Empty;
            this.shortTitle = String.Empty;
            this.longTitle = String.Empty;
            this.email = String.Empty;
            this.copyright = String.Empty;
            this.disclaimer = String.Empty;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Federation</b> object to create the deep copy from.</param>
        private void CopyMembers(Federation old)
        {
            this.queryFactory = old.queryFactory;
            this.fileFormatFactory = old.fileFormatFactory;
            this.streamFactory = old.streamFactory;
            this.shortTitle = old.shortTitle;
            this.longTitle = old.longTitle;
            this.email = old.email;
            this.copyright = old.copyright;
            this.disclaimer = old.disclaimer;
        }

        public override object Clone()
        {
            return new Federation(this);
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
                typeof(JobDefinition),
            };
        }

        #endregion

    }
}
