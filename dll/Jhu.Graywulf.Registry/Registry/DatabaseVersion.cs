/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Database Version</b> entity
    /// </summary>
    public partial class DatabaseVersion : Entity
    {
        public enum ReferenceType : int
        {
            ServerVersion = 1,
        }

        #region Member Variables

        // --- Background storage for properties ---
        private float sizeMultiplier;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.DatabaseVersion; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Federation; }
        }

        [DBColumn]
        public float SizeMultiplier
        {
            get { return sizeMultiplier; }
            set { sizeMultiplier = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Database Definition</b> object to which this <b>Database Version</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public DatabaseDefinition DatabaseDefinition
        {
            get { return (DatabaseDefinition)ParentReference.Value; }
        }

        /// <summary>
        /// Gets the reference object to the default target <b>Server Version</b> of this <b>Database Version</b>
        /// </summary>
        [XmlIgnore]
        public EntityReference<ServerVersion> ServerVersionReference
        {
            get { return (EntityReference<ServerVersion>)EntityReferences[(int)ReferenceType.ServerVersion]; }
        }

        /// <summary>
        /// Gets the default target <b>Server Version</b>.
        /// </summary>
        [XmlIgnore]
        public ServerVersion ServerVersion
        {
            get { return ServerVersionReference.Value; }
            set { ServerVersionReference.Value = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("ServerVersion")]
        public string ServerVersion_ForXml
        {
            get { return ServerVersionReference.Name; }
            set { ServerVersionReference.Name = value; }
        }

        [XmlIgnore]
        public Dictionary<string, UserDatabaseInstance> UserDatabaseInstances
        {
            get { return GetChildren<UserDatabaseInstance>(); }
            set { SetChildren<UserDatabaseInstance>(value); }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public DatabaseVersion()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Database Version</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public DatabaseVersion(RegistryContext context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DatabaseVersion(DatabaseDefinition parent)
            : base(parent.RegistryContext, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Database Version</b> objects.
        /// </summary>
        /// <param name="old">The <b>Database Version</b> to copy from.</param>
        public DatabaseVersion(DatabaseVersion old)
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
            this.sizeMultiplier = 1.0f;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Database Version</b> object to create the deep copy from.</param>
        private void CopyMembers(DatabaseVersion old)
        {
            this.sizeMultiplier = old.sizeMultiplier;
        }

        internal override bool CompareMembers(Entity other)
        {
            bool eq = base.CompareMembers(other);
            var o = other as DatabaseVersion;

            eq &= this.sizeMultiplier == o.sizeMultiplier;

            return eq;
        }

        internal override void UpdateMembers(Entity other)
        {
            base.UpdateMembers(other);
            var o = other as DatabaseVersion;
            CopyMembers(o);
        }

        public override object Clone()
        {
            return new DatabaseVersion(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<ServerVersion>((int)ReferenceType.ServerVersion),
            };
        }

        protected override EntityType[] CreateChildTypes()
        {
            return new EntityType[] 
            {
                EntityType.UserDatabaseInstance,
            };
        }

        #endregion

        public DatabaseInstance GetUserDatabaseInstance(User user)
        {
            var ef = new EntityFactory(RegistryContext);

            var udis = ef.FindConnection<UserDatabaseInstance>(this, user, (int)UserDatabaseInstance.ReferenceType.User);
            var udi = udis.FirstOrDefault();

            if (udi == null)
            {
                return null;
            }
            else
            {
                return udi.DatabaseInstance;
            }
        }
    }
}
