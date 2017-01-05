/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using System.Xml;
using System.Reflection;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Class that acts as the base class to all Cluster Schema Description classes.
    /// </summary>
    /// <remarks>
    /// It contains the common
    /// properties for hierarchy handling, database IO and concurrency handling. Do not use this class
    /// directly, use inherited classes instead. About the entity hierarchy, refer to the Developer's Guide.
    /// </remarks>
    public partial class Entity : ContextObject, ICloneable
    {
        public static StringComparer StringComparer
        {
            get { return StringComparer.InvariantCultureIgnoreCase; }
        }

        #region Member Variables

        private bool isExisting;
        private bool isDeserializing;

        // --- Background storage for properties ---
        private Guid guid;
        private long concurrencyVersion;
        private EntityReference<Entity> parentReference;
        private EntityType entityTypeInternal;
        private int number;
        private string name;
        private string displayName;
        private string version;
        private bool system;
        private bool hidden;
        private bool readOnly;
        private bool primary;
        private bool deleted;
        private Guid lockOwner;
        private RunningState runningState;
        private int alertState;
        private DeploymentState deploymentState;
        private Guid userGuidOwner;
        private DateTime dateCreated;
        private Guid userGuidCreated;
        private DateTime dateModified;
        private Guid userGuidModified;
        private DateTime dateDeleted;
        private Guid userGuidDeleted;
        private ParameterCollection settings;
        private string comments;

        private string fullyQualifiedName;

        // --- Background storage for navigation properties ---
        private Dictionary<int, IEntityReference> entityReferences;
        private bool isEntityReferencesLoaded;

        private Dictionary<EntityType, System.Collections.IDictionary> childEntities;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Returns <b>true</b> when there is a persisted version of this entity in the database.
        /// </summary>
        public bool IsExisting
        {
            get { return isExisting; }
        }

        internal bool IsDeserializing
        {
            get { return isDeserializing; }
            set { isDeserializing = value; }
        }
        
        /// <summary>
        /// Globally unique identifier of the entity, acts like the primary key in the database. When creating
        /// a new entity has to have the values of <c>Guid.Empty</c>. A new Guid is generated when the entity
        /// is first time saved to the database calling the <see cred="Save" /> method.
        /// </summary>
        [XmlIgnore]
        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        /// <summary>
        /// Read-only property returning the type of the entity. Its numerical value is stored in the database.
        /// The value of this property is set by the constructor of the inherited classes.
        /// </summary>
        [XmlIgnore]
        public virtual EntityType EntityType
        {
            get { return entityTypeInternal; }
            private set { entityTypeInternal = value; }
        }

        /// <summary>
        /// Gets or sets the value of the variable used for the optimistic concurrency model implementation.
        /// Refer to the Developer's Guide for more information.
        /// </summary>
        [XmlIgnore]
        public long ConcurrencyVersion
        {
            get { return concurrencyVersion; }
            set { concurrencyVersion = value; }
        }

        /// <summary>
        /// Gets the parent of the entity in the entity hierarchy.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading. Most inherited classes implement a strongly typed version
        /// of this property name according to the type of the parent entity.
        /// </remarks>
        [XmlIgnore]
        public Entity Parent
        {
            get { return parentReference.Value; }
            internal set { parentReference.Value = value; }
        }

        /// <summary>
        /// Gets a reference object to the parent of the entity.
        /// </summary>
        [XmlIgnore]
        public EntityReference<Entity> ParentReference
        {
            get { return parentReference; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("Parent")]
        public string Parent_ForXml
        {
            get { return parentReference.Name; }
            set { parentReference.Name = value; }
        }

        /// <summary>
        /// Returns an enumerator to all the referenced entities;
        /// </summary>
        [XmlIgnore]
        internal Dictionary<int, IEntityReference> EntityReferences
        {
            get
            {
                if (!isEntityReferencesLoaded)
                {
                    LoadEntityReferences();
                }

                return entityReferences;
            }
        }

        internal bool IsEntityReferencesLoaded
        {
            get { return isEntityReferencesLoaded; }
            set { isEntityReferencesLoaded = value; }
        }

        /// <summary>
        /// Gets the entity's ordinal number in its hierarchy level.
        /// </summary>
        /// <remarks>This property is an ordinal number generated on the basis of the hierarchical model.
        /// Child entities of an entity are numbered from 0 to the total number of the entities - 1 of a certain type.
        /// That is, if an entity has child entities of different type, each set will be numbered individually.
        /// Consistent ordering and numbering of child entities is essential in the automatic logical–physical
        /// mapping scenarios. Refer to the Developer's Guide for more information.</remarks>
        [XmlIgnore]
        public int Number
        {
            get { return number; }
        }

        /// <summary>
        /// Gets or sets the property of an entity that is diplayed in the Cluster Management Console.
        /// </summary>
        /// <remarks>
        /// It is usually different from internal identifiers and is for enabling the system to display human readable titles.
        /// </remarks>
        [XmlAttribute]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the name of the entity which is occasionally displayed on the user interface.
        /// </summary>
        [XmlAttribute]
        [DefaultValue("")]
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        /// <summary>
        /// Gets or sets the version number associated with the entity.
        /// </summary>
        /// <remarks>
        /// Versioning of entities is not implemented inside the Cluster Management System, it is a placeholder
        /// for cluster builders.
        /// </remarks>
        [DefaultValue("")]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// Gets or sets the system flag. Reserved for future use.
        /// </summary>
        [DefaultValue(false)]
        public bool System
        {
            get { return system; }
            set { system = value; }
        }

        /// <summary>
        /// Gets or sets the value of the flag that determines whether the entity is diplayed in the
        /// Cluster Management Console.
        /// </summary>
        [DefaultValue(false)]
        public bool Hidden
        {
            get { return hidden; }
            set { hidden = value; }
        }

        /// <summary>
        /// Gets or sets the value of the flag that determines whether the entity is editable
        /// from the Cluster Management Console. Does not affect the behaviour of the class library,
        /// read-only entities can be saved anytime using the <see cref="Save()" /> method.
        /// </summary>
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        [XmlIgnore]
        public bool Primary
        {
            get { return primary; }
        }

        /// <summary>
        /// Gets the value indicating if the entity was deleted. 
        /// </summary>
        /// <remarks>
        /// Deleted entities are not removed from the database, only flagged. This is for system tracking purposes.
        /// </remarks>
        [XmlIgnore]
        public bool Deleted
        {
            get { return deleted; }
        }

        /// <summary>
        /// Contains the GUID of the Workflow Instance having a lock on the <b>Entity</b>.
        /// </summary>
        /// <remarks>
        /// The values is GUID.Empty if there is no activity holding a lock on the object.
        /// Use the <see cref="IsLocked"/> property to test if object is locked or not.
        /// </remarks>
        [XmlIgnore]
        public Guid LockOwner
        {
            get { return lockOwner; }
            set { lockOwner = value; }
        }

        /// <summary>
        /// Gets or sets the value of the entities running state. Used mainly for entities refering to hardware
        /// resources.
        /// </summary>
        [DefaultValue(RunningState.Unknown)]
        public RunningState RunningState
        {
            get { return runningState; }
            set { runningState = value; }
        }

        /// <summary>
        /// Gets or sets the value the entities alert state.
        /// </summary>
        /// <remarks>
        /// Alert state is used to inform system administrators about the potential problems in the system.
        /// The value of this flag is displayed as an icon the the Cluster Management Console.
        /// </remarks>
        [DefaultValue(0)]
        public int AlertState
        {
            get { return alertState; }
            set { alertState = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating the deployment status of hardware, logical or mapping entities.
        /// </summary>
        [DefaultValue(DeploymentState.New)]
        public DeploymentState DeploymentState
        {
            get { return deploymentState; }
            set { deploymentState = value; }
        }

        /// <summary>
        /// Gets the Guid of the user that owns the entity.
        /// </summary>
        /// <remarks>
        /// This property is reserved for future use in a security system.
        /// </remarks>
        [XmlIgnore]
        public Guid UserGuidOwner
        {
            get { return userGuidOwner; }
        }

        /// <summary>
        /// Gets the date and time when the entity was created.
        /// </summary>
        [XmlIgnore]
        public DateTime DateCreated
        {
            get { return dateCreated; }
        }

        /// <summary>
        /// Gets the Guid of the user who created the entity.
        /// </summary>
        [XmlIgnore]
        public Guid UserGuidCreated
        {
            get { return userGuidCreated; }
        }

        /// <summary>
        /// Gets the date and time when the entity was modified.
        /// </summary>
        [XmlIgnore]
        public DateTime DateModified
        {
            get { return dateModified; }
        }

        /// <summary>
        /// Gets the Guid of the user who modified the entity.
        /// </summary>
        [XmlIgnore]
        public Guid UserGuidModified
        {
            get { return userGuidModified; }
        }

        /// <summary>
        /// Gets the date and time when the entity was deleted.
        /// </summary>
        /// <remarks>
        /// Contains invalid value if the value of the <see cref="Deleted" /> property is <b>true</b>.
        /// </remarks>
        [XmlIgnore]
        public DateTime DateDeleted
        {
            get { return dateDeleted; }
        }

        /// <summary>
        /// Gets the Guid of the user who deleted the entity.
        /// </summary>
        [XmlIgnore]
        public Guid UserGuidDeleted
        {
            get { return userGuidDeleted; }
        }

        [XmlIgnore]
        public ParameterCollection Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        /// <summary>
        /// Gets or sets an XML string containing additional settings.
        /// </summary>
        /// <remarks>
        /// This property is for extendability purposes.
        /// </remarks>
        [XmlArray("Settings")]
        [XmlArrayItem(typeof(Parameter))]
        [DefaultValue(null)]
        public Parameter[] Settings_ForXml
        {
            get { return settings.GetAsArray(); }
            set { settings = new ParameterCollection(value); }
        }

        /// <summary>
        /// Gets or sets the value of a free text field for annotations by the
        /// system administrators.
        /// </summary>
        [DefaultValue("")]
        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }

        /// <summary>
        /// Gets the Group mask of the entity.
        /// </summary>
        [XmlIgnore]
        public virtual EntityGroup EntityGroup
        {
            get { return EntityGroup.Unknown; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public Entity()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor for creating a new entity and setting object context.
        /// </summary>
        /// <remarks>
        /// Override this in derived classes and pass the object context to it.
        /// </remarks>
        /// <param name="context">An object context class containing session information.</param>
        public Entity(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        protected Entity(Context context, Entity parent)
            : base(context)
        {
            InitializeMembers(new StreamingContext());

            this.parentReference.Value = parent;
        }

        /// <summary>
        /// Copy contructor for doing deep copy of entity classes.
        /// </summary>
        /// <param name="old">The entity to copy from.</param>
        public Entity(Entity old)
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
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            var now = DateTime.Now;

            this.isExisting = false;
            this.isDeserializing = false;

            this.guid = Guid.Empty;
            this.concurrencyVersion = 0;
            this.parentReference = new EntityReference<Entity>(this);
            this.entityTypeInternal = EntityType.Unknown;
            this.number = 0;
            this.name = string.Empty;
            this.displayName = string.Empty;
            this.version = string.Empty;
            this.system = false;
            this.hidden = false;
            this.readOnly = false;
            this.primary = false;
            this.deleted = false;
            this.lockOwner = Guid.Empty;
            this.runningState = 0;
            this.alertState = 0;
            this.deploymentState = DeploymentState.New;
            this.userGuidOwner = Guid.Empty;
            this.dateCreated = now;
            this.userGuidCreated = Guid.Empty;
            this.dateModified = now;
            this.userGuidModified = Guid.Empty;
            this.dateDeleted = DateTime.MinValue;
            this.userGuidDeleted = Guid.Empty;
            this.settings = new ParameterCollection();
            this.comments = string.Empty;

            this.fullyQualifiedName = null;

            InitializeEntityReferences();
            this.isEntityReferencesLoaded = false;

            InitializeChildTypes();
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">An entity object to create the deep copy from.</param>
        private void CopyMembers(Entity old)
        {
            this.guid = old.guid;
            this.concurrencyVersion = old.concurrencyVersion;
            this.parentReference = new EntityReference<Entity>(this, old.parentReference);
            this.EntityType = old.EntityType;   // always use property!
            this.number = old.number;
            this.name = old.name;
            this.displayName = old.displayName;
            this.version = old.version;
            this.system = old.system;
            this.hidden = old.hidden;
            this.readOnly = old.readOnly;
            this.primary = old.primary;
            this.deleted = old.deleted;
            this.lockOwner = old.lockOwner;
            this.runningState = old.runningState;
            this.alertState = old.alertState;
            this.deploymentState = old.deploymentState;
            this.userGuidOwner = old.userGuidOwner;
            this.dateCreated = old.dateCreated;
            this.userGuidCreated = old.userGuidCreated;
            this.dateModified = old.dateModified;
            this.userGuidModified = old.userGuidModified;
            this.dateDeleted = old.dateDeleted;
            this.userGuidDeleted = old.userGuidDeleted;
            this.settings = new ParameterCollection(old.settings);
            this.comments = old.comments;

            this.fullyQualifiedName = old.fullyQualifiedName;

            CopyEntityReferences(old);
            InitializeChildTypes();
        }

        internal virtual bool CompareMembers(Entity other)
        {
            bool eq = true;

            eq &= Entity.StringComparer.Compare(this.name, other.name) == 0;
            eq &= Entity.StringComparer.Compare(this.displayName, other.displayName) == 0;
            eq &= Entity.StringComparer.Compare(this.version, other.version) == 0;
            eq &= this.system == other.system;
            eq &= this.hidden == other.hidden;
            eq &= this.readOnly == other.readOnly;
            eq &= this.primary == other.primary;
            eq &= this.deleted == other.deleted;
            eq &= this.runningState == other.runningState;
            eq &= this.alertState == other.alertState;
            eq &= this.deploymentState == other.deploymentState;
            eq &= this.settings.CompareItems(other.settings);
            eq &= this.comments == other.comments;

            return eq;
        }

        internal virtual void UpdateMembers(Entity other)
        {
            this.name = other.name;
            this.displayName = other.displayName;
            this.version = other.version;
            this.system = other.system;
            this.hidden = other.hidden;
            this.readOnly = other.readOnly;
            this.primary = other.primary;
            this.deleted = other.deleted;
            this.runningState = other.runningState;
            this.alertState = other.alertState;
            this.deploymentState = other.deploymentState;
            this.settings = new ParameterCollection(other.settings);
            this.comments = other.comments;
        }

        public virtual object Clone()
        {
            return new Entity(this);
        }

        #endregion
        #region Navigation Functions

        protected virtual IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[0];
        }

        private void InitializeEntityReferences()
        {
            this.entityReferences = new Dictionary<int, IEntityReference>();

            foreach (var er in CreateEntityReferences())
            {
                entityReferences.Add(er.ReferenceType, er);
                er.ReferencingEntity = this;
            }
        }

        private void CopyEntityReferences(Entity old)
        {
            this.entityReferences = new Dictionary<int, IEntityReference>();
            this.isEntityReferencesLoaded = old.isEntityReferencesLoaded;

            foreach (var er in old.entityReferences.Values)
            {
                var ner = er.Clone();

                entityReferences.Add(ner.ReferenceType, ner);
                ner.ReferencingEntity = this;
            }
        }

        protected virtual EntityType[] CreateChildTypes()
        {
            return new EntityType[0];
        }

        private void InitializeChildTypes()
        {
            this.childEntities = new Dictionary<EntityType, System.Collections.IDictionary>();

            foreach (EntityType t in CreateChildTypes())
            {
                childEntities.Add(t, null);
            }
        }

        protected Dictionary<string, T> GetChildren<T>()
        {
            return (Dictionary<string, T>)childEntities[Constants.EntityTypeMap[typeof(T)]];
        }

        protected void SetChildren<T>(Dictionary<string, T> value)
        {
            childEntities[Constants.EntityTypeMap[typeof(T)]] = value;
        }

        public IEnumerable<Entity> EnumerateChildren(EntityType entityType)
        {
            return childEntities[entityType].Values.Cast<Entity>();
        }

        /// <summary>
        /// Gets an <b>IEnumerable&lt;<see cref="Entity" />&gt;</b> interface to the entity's child entities.
        /// </summary>
        /// <remarks>
        /// This method doesn't load the <b>Child Entities</b>. Use the <see cref="Entity.LoadAllChildren()" /> or <see cref="Entity.LoadAllChildren(bool)"/> method to load child
        /// entities first. Only implemented in derived classes.
        /// </remarks>
        public IEnumerable<Entity> EnumerateAllChildren()
        {
            foreach (EntityType t in childEntities.Keys)
            {
                foreach (object o in childEntities[t].Values)
                {
                    yield return (Entity)o;
                }
            }
        }

        /// <summary>
        /// Gets an <b>IEnumerable&lt;<see cref="Entity" />&gt;</b> interfate to the entity's parents up to the
        /// <b>Cluter</b> entity which doesn't have a parent.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Entity> EnumerateParents(bool includeItself)
        {
            if (includeItself) yield return this;

            Entity e = this;
            while (e.Parent != null)
            {
                yield return e.Parent;
                e = e.Parent;
            }
        }

        #endregion

        /// <summary>
        /// Computes the fully qualified name of the entity.
        /// </summary>
        /// <returns>The fully qualified name of the entity.</returns>
        /// <remarks>
        /// This function loads the ascendants of the entity so when calling
        /// a valid context should be used.
        /// </remarks>
        public string GetFullyQualifiedName()
        {
            if (fullyQualifiedName == null)
            {
                if (isExisting)
                {
                    var names = LoadAscendantNames();
                    var fqn = "";

                    for (int i = 0; i < names.Length; i++)
                    {
                        if (fqn != "")
                        {
                            fqn += Constants.EntityNameSeparator;
                        }

                        fqn += names[i];
                    }

                    fullyQualifiedName = EntityType.ToString() + Constants.EntityTypeSeparator + fqn;
                }
                else if (!parentReference.IsEmpty && !String.IsNullOrEmpty(name))
                {
                    fullyQualifiedName = EntityFactory.CombineName(EntityType, parentReference.Name, name);
                }
                else if (EntityType == EntityType.Cluster)
                {
                    fullyQualifiedName = EntityFactory.CombineName(EntityType.Cluster, name);
                }
            }

            return fullyQualifiedName;
        }

        /// <summary>
        /// Set the fully qualified name of the entity by bypassing
        /// access to the registry
        /// </summary>
        /// <param name="fullyQualifiedName"></param>
        public void SetFullyQualifiedName(string fullyQualifiedName)
        {
            this.fullyQualifiedName = fullyQualifiedName;
        }

        public override string ToString()
        {
            return GetFullyQualifiedName();
        }
    }
}
