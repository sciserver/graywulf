/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Slice</b> entity.
    /// </summary>
    public partial class MyDBDatabaseInstance : Entity
    {
        public enum ReferenceType : int
        {
            Federation = 1,
            DatabaseDefinition = 2,
            DatabaseInstance = 3
        }

        #region Member Variables


        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public Federation Federation
        {
            get { return FederationReference.Value; }
            set { FederationReference.Value = value; }
        }

        [XmlIgnore]
        public DatabaseDefinition DatabaseDefinition
        {
            get { return DatabaseDefinitionReference.Value; }
            set { DatabaseDefinitionReference.Value = value; }
        }

        [XmlIgnore]
        public DatabaseInstance DatabaseInstance
        {
            get { return DatabaseInstanceReference.Value; }
            set { DatabaseInstanceReference.Value = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Database Definition</b> object to which this <b>Slice</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        public User User
        {
            get
            {
                return (User)ParentReference.Value;
            }
        }

        [XmlIgnore]
        public EntityReference<Federation> FederationReference
        {
            get { return (EntityReference<Federation>)EntityReferences[(int)ReferenceType.Federation]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("Federation")]
        public string Federation_ForXml
        {
            get { return FederationReference.Name; }
            set { FederationReference.Name = value; }
        }

        [XmlIgnore]
        public EntityReference<DatabaseDefinition> DatabaseDefinitionReference
        {
            get { return (EntityReference<DatabaseDefinition>)EntityReferences[(int)ReferenceType.DatabaseDefinition]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("DatabaseDefinition")]
        public string DatabaseDefinition_ForXml
        {
            get { return DatabaseDefinitionReference.Name; }
            set { DatabaseDefinitionReference.Name = value; }
        }

        [XmlIgnore]
        public EntityReference<DatabaseInstance> DatabaseInstanceReference
        {
            get { return (EntityReference<DatabaseInstance>)EntityReferences[(int)ReferenceType.DatabaseInstance]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("DatabaseInstance")]
        public string DatabaseInstance_ForXml
        {
            get { return DatabaseInstanceReference.Name; }
            set { DatabaseInstanceReference.Name = value; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public MyDBDatabaseInstance()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Slice</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public MyDBDatabaseInstance(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public MyDBDatabaseInstance(Context context, DatabaseDefinition parent)
            : base(context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Slice</b> objects.
        /// </summary>
        /// <param name="old">The <b>Slice</b> to copy from.</param>
        public MyDBDatabaseInstance(MyDBDatabaseInstance old)
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
            base.EntityType = EntityType.MyDBDatabaseInstance;
            base.EntityGroup = EntityGroup.Layout;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Slice</b> object to create the deep copy from.</param>
        private void CopyMembers(MyDBDatabaseInstance old)
        {

        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<Federation>((int)ReferenceType.Federation),
                new EntityReference<DatabaseDefinition>((int)ReferenceType.DatabaseDefinition),
                new EntityReference<DatabaseInstance>((int)ReferenceType.DatabaseInstance),
            };
        }

        #endregion
    }
}
