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
        /// Guid of the referenced entity
        /// </summary>
        private Guid guid;

        /// <summary>
        /// Name of the referenced entity
        /// </summary>
        private string name;

        /// <summary>
        /// Parent object of the referenced entity that
        /// has a valid context
        /// </summary>
        [NonSerialized]
        protected IContextObject referencingObject;

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

        /// <summary>
        /// Returns true if the reference is empty.
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
                if (guid == Guid.Empty)
                {
                    if (value == null && !IsEmpty)
                    {
                        LoadEntity();   
                    }

                    guid = value.Guid;
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
                if (name == null)
                {
                    if (value == null && !IsEmpty)
                    {
                        LoadEntity();
                    }

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

        public IContextObject ReferencingObject
        {
            get { return referencingObject; }
            set { referencingObject = value; }
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
                if (this.value == null && !IsEmpty)
                {
                    LoadEntity();
                }

                // Update context before returning the entity
                if (this.value != null && this.referencingObject != null)
                {
                    this.value.Context = this.referencingObject.Context;
                }

                return this.value;
            }
            set
            {
                this.value = value;
                if (this.value != null)
                {
                    this.guid = this.value.Guid;
                    
                    // TODO: remove this and use lazy-loading of full name
                    // for performance reasons
                    //this.name = this.value.GetFullyQualifiedName();
                    this.name = null;
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
        Guid IEntityReference.ReferencedEntityGuid
        {
            get { return Guid; }
            set { Guid = value; }
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
            this.guid = Guid.Empty;
            this.name = null;
            this.referencingObject = null;
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

        public void LoadEntity()
        {
            Context context;
            if (referencingObject == null)
            {
                context = null;
            }
            else
            {
                context = referencingObject.Context;
            }

            if (this.value == null)
            {
                var ef = new EntityFactory(context);

                if (this.guid != Guid.Empty)
                {
                    this.value = ef.LoadEntity<T>(this.guid);

                    // TODO: remove this and use lazy loading for performance
                    //this.name = this.value.GetFullyQualifiedName();
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

                // Update IDs
                this.guid = this.value.Guid;

                // TODO: remove this and use lazy-loading for performance
                // this.name = this.value.GetFullyQualifiedName();
                this.name = null;
            }
        }
    }
}
