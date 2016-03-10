/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// The class implements a wrapper around the entity GUID and name used for navigation
    /// properties for entity cross references.
    /// </summary>
    /// <remarks>
    /// The class supports lazy loading of the referenced entity by its GUID or fully
    /// qualified name. Always use the GUID property to set the referenced entity.
    /// </remarks>
    /// <typeparam name="T">Type of the referenced entity</typeparam>
    [Serializable]
    [DataContract(Namespace = "")]
    public class EntityReference<T> : IEntityReference, ICloneable
        where T : Entity, new()
    {
        #region Member Variables

        /// <summary>
        /// Parent object of the referenced entity that
        /// has a valid context
        /// </summary>
        [NonSerialized]
        protected IContextObject referencingObject;

        /// <summary>
        /// Guid of the referenced entity
        /// </summary>
        private Guid guid;

        /// <summary>
        /// Name of the referenced entity
        /// </summary>
        private string name;

        /// <summary>
        /// The referenced entity itself.
        /// </summary>
        [NonSerialized]
        protected T value;

        /// <summary>
        /// Type of the reference, set by entities
        /// </summary>
        private int referenceType;

        #endregion
        #region Member Access Properties

        public IContextObject ReferencingObject
        {
            get { return referencingObject; }
            set { referencingObject = value; }
        }

        /// <summary>
        /// Returns true if the reference is empty, i.e. it
        /// has no value in the name, guid and value fields.
        /// </summary>
        [IgnoreDataMember]
        public bool IsEmpty
        {
            get { return guid == Guid.Empty && name == null && value == null; }
        }

        /// <summary>
        /// Gets or sets the GUID of the referenced entity.
        /// </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public Guid Guid
        {
            get
            {
                if (IsEmpty)
                {
                    return Guid.Empty;
                }
                else if (guid == Guid.Empty)
                {
                    EnsureEntityLoaded();

                    if (value != null)
                    {
                        guid = value.Guid;
                    }
                }

                return guid;
            }
            set
            {
                guid = value;
                this.value = null;
            }
        }

        /// <summary>
        /// Gets or sets the Name of the referenced entity.
        /// </summary>
        [DataMember]
        [XmlText]
        public string Name
        {
            get
            {
                if (IsEmpty)
                {
                    return null;
                }
                else if (name == null)
                {
                    EnsureEntityLoaded();

                    if (value != null)
                    {
                        name = value.GetFullyQualifiedName();
                    }
                }

                return name;
            }
            set
            {
                name = value;
                this.value = null;
            }
        }

        /// <summary>
        /// Gets or sets the referenced entity.
        /// </summary>
        /// <remarks>
        /// By setting the property the values of the <see cref="Guid"/> and <see cref="Name"/> properties
        /// are updated. This property does lazy loading, a valid context is required.
        /// </remarks>
        [IgnoreDataMember]
        [XmlIgnore]
        public T Value
        {
            get
            {
                EnsureEntityLoaded();
                return this.value;
            }
            set
            {
                this.value = value;

                if (this.value != null)
                {
                    this.guid = this.value.Guid;
                    this.name = value.GetFullyQualifiedName(); ;
                }
                else
                {
                    this.guid = Guid.Empty;
                    this.name = null;
                }
            }
        }

        #endregion
        #region IEntityReference implementation

        [XmlIgnore]
        Entity IEntityReference.ReferencingEntity
        {
            get { return (Entity)ReferencingObject; }
            set { ReferencingObject = value; }
        }

        [XmlIgnore]
        int IEntityReference.ReferenceType
        {
            get { return referenceType; }
        }

        [XmlIgnore]
        object IEntityReference.Value
        {
            get { return Value; }
            set { Value = (T)value; }
        }

        #endregion
        #region Constructors and initializers

        public EntityReference(IContextObject referencingObject)
        {
            InitializeMembers(new StreamingContext());

            this.referencingObject = referencingObject;
        }

        /// <summary>
        /// The constructor sets the referenced entity to the one passed as parameter.
        /// </summary>
        /// <param name="referencingEntity">Entity to reference</param>
        public EntityReference(int referenceType)
        {
            InitializeMembers(new StreamingContext());

            this.referenceType = referenceType;
        }

        /// <summary>
        /// The copy constructor that creates a deep copy of the
        /// <b>ReferencedEntity</b> class passes as parameter.
        /// </summary>
        /// <param name="old">The original object to copy from.</param>
        public EntityReference(IContextObject referencingObject, EntityReference<T> old)
        {
            CopyMembers(old);

            this.referencingObject = referencingObject;
        }

        /// <summary>
        /// Initializes private member variables to their default values.
        /// </summary>
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.referencingObject = null;
            this.guid = Guid.Empty;
            this.name = null;
            this.value = null;
            this.referenceType = -1;
        }

        /// <summary>
        /// Copies private member variables from another object.
        /// </summary>
        /// <remarks>
        /// Called by the copy constructor.
        /// </remarks>
        /// <param name="old">The original object to copy from.</param>
        private void CopyMembers(EntityReference<T> old)
        {
            this.guid = old.guid;
            this.name = old.name;
            this.referencingObject = null;
            this.value = old.value;
            this.referenceType = old.referenceType;
        }

        public object Clone()
        {
            return new EntityReference<T>(this.referencingObject, this);
        }

        IEntityReference IEntityReference.Clone()
        {
            return new EntityReference<T>(this.referencingObject, this);
        }

        #endregion


        /// <summary>
        /// Loads the entity from the registry using the context of the
        /// referencing object.
        /// </summary>
        internal void EnsureEntityLoaded()
        {
            if (!IsEmpty)
            {
                // Get a fresh context from the referencing object

                Context context = null;

                if (referencingObject != null && referencingObject.Context != null)
                {
                    context = referencingObject.Context;
                }
                else if (value != null && value.Context != null)
                {
                    context = this.value.Context;
                }

                // Load or update entity
                // A null value means the referenced entity has not been loaded yet.
                // In this case, load it based on GUID or name.

                if (this.value == null)
                {
                    var ef = new EntityFactory(context);

                    if (this.guid != Guid.Empty)
                    {
                        this.value = ef.LoadEntity<T>(this.guid);
                        this.name = null;
                    }
                    else if (this.name != null)
                    {
                        this.value = ef.LoadEntity<T>(this.name);
                        this.guid = this.value.Guid;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    // Update context
                    this.value.Context = context;
                }
            }
        }
    }
}
