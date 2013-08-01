/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Distributed Partitioned View</b> entity
    /// </summary>
    public partial class DistributedPartitionedView : Entity
    {
        public enum ReferenceType : int
        {
            DatabaseVersion = 1,
            ReferencedDatabaseDefinition = 2,
            ReferencedDatabaseVersion = 3
        }

        #region Member Variables

        // --- Background storage for properties ---
        private string viewSchema;
        private string viewName;
        private EntityReference<DatabaseVersion> databaseVersionReference;
        private EntityReference<DatabaseDefinition> referencedDatabaseDefinitionReference;
        private string referencedTableSchema;
        private string referencedTableName;
        private EntityReference<DatabaseVersion> referencedDatabaseVersionReference;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the schema of the view created in the main database.
        /// </summary>
        public string ViewSchema
        {
            get { return viewSchema; }
            set { viewSchema = value; }
        }

        /// <summary>
        /// Gets or sets the name of the view created in the main database.
        /// </summary>
        public string ViewName
        {
            get { return viewName; }
            set { viewName = value; }
        }

        /// <summary>
        /// Gets the reference object to the database version of the
        /// main database.
        /// </summary>
        [XmlIgnore]
        public EntityReference<DatabaseVersion> DatabaseVersionReference
        {
            get { return databaseVersionReference; }
        }

        /// <summary>
        /// Gets the reference object to the the database definition of the
        /// referenced database.
        /// </summary>
        /// <remarks>
        /// The main database (in which the view is created) is always the
        /// parent entity.
        /// </remarks>
        [XmlIgnore]
        public EntityReference<DatabaseDefinition> ReferencedDatabaseDefinitionReference
        {
            get { return referencedDatabaseDefinitionReference; }
        }

        /// <summary>
        /// Gets or sets the schema of the table in the referenced database.
        /// </summary>
        public string ReferencedTableSchema
        {
            get { return referencedTableSchema; }
            set { referencedTableSchema = value; }
        }

        /// <summary>
        /// Gets or sets the name of the table in the referenced database.
        /// </summary>
        public string ReferencedTableName
        {
            get { return referencedTableName; }
            set { referencedTableName = value; }
        }

        /// <summary>
        /// Gets the reference object to the database version of the referenced database.
        /// </summary>
        [XmlIgnore]
        public EntityReference<DatabaseVersion> ReferencedDatabaseVersionReference
        {
            get { return referencedDatabaseVersionReference; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Database Definition</b> object to which this <b>Distributed Partitioned View</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        public DatabaseDefinition DatabaseDefinition
        {
            get { return (DatabaseDefinition)parentReference.Value; }
        }

        /// <summary>
        /// Gets the database version of the main database.
        /// </summary>
        [XmlIgnore]
        public DatabaseVersion DatabaseVersion
        {
            get { return databaseVersionReference.Value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("DatabaseVersion")]
        public string DatabaseVersion_ForXml
        {
            get { return databaseVersionReference.Name; }
            set { databaseVersionReference.Name = value; }
        }

        /// <summary>
        /// Gets the reference object to the referenced database definition
        /// </summary>
        [XmlIgnore]
        public DatabaseDefinition ReferencedDatabaseDefinition
        {
            get { return referencedDatabaseDefinitionReference.Value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("ReferencedDatabaseDefinition")]
        public string ReferencedDatabaseDefinition_ForXml
        {
            get { return referencedDatabaseDefinitionReference.Name; }
            set { referencedDatabaseDefinitionReference.Name = value; }
        }

        /// <summary>
        /// Gets the referenced database version
        /// </summary>
        [XmlIgnore]
        public DatabaseVersion ReferencedDatabaseVersion
        {
            get { return referencedDatabaseVersionReference.Value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("ReferencedDatabaseVersion")]
        public string ReferencedDatabaseVersion_ForXml
        {
            get { return referencedDatabaseVersionReference.Name; }
            set { referencedDatabaseVersionReference.Name = value; }
        }

        #endregion
        #region Validation Properties
        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public DistributedPartitionedView()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>DistributedPartitionedView</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public DistributedPartitionedView(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        public DistributedPartitionedView(Context context, DatabaseDefinition parent)
            : base(context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>DistributedPartitionedView</b> objects.
        /// </summary>
        /// <param name="old">The <b>DistributedPartitionedView</b> to copy from.</param>
        public DistributedPartitionedView(DistributedPartitionedView old)
            : base(old)
        {
            CopyMembers(old);
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes member variables to their initial values.
        /// </summary>
        /// <remarks>
        /// This function is called by the contructors.
        /// </remarks>
        private void InitializeMembers()
        {
            base.EntityType = EntityType.DistributedPartitionedView;
            base.EntityGroup = EntityGroup.Federation;

            this.viewSchema = string.Empty;
            this.viewName = string.Empty;
            this.databaseVersionReference = new EntityReference<DatabaseVersion>(this);
            this.referencedDatabaseDefinitionReference = new EntityReference<DatabaseDefinition>(this);
            this.referencedTableSchema = string.Empty;
            this.referencedTableName = string.Empty;
            this.referencedDatabaseVersionReference = new EntityReference<DatabaseVersion>(this);
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Cluster</b> object to create the deep copy from.</param>
        private void CopyMembers(DistributedPartitionedView old)
        {
            this.viewSchema = old.viewSchema;
            this.viewName = old.viewName;
            this.databaseVersionReference = new EntityReference<DatabaseVersion>(old.databaseVersionReference);
            this.referencedDatabaseDefinitionReference = new EntityReference<DatabaseDefinition>(old.referencedDatabaseDefinitionReference);
            this.referencedTableSchema = old.referencedTableSchema;
            this.referencedTableName = old.referencedTableName;
            this.referencedDatabaseVersionReference = new EntityReference<DatabaseVersion>(old.referencedDatabaseVersionReference);
        }

        #endregion
    }
}
