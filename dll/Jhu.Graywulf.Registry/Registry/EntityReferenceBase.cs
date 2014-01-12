/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// The class implements a wrapper around the entity GUID and used for navigation
    /// properties for entity cross references.
    /// </summary>
    /// <remarks>
    /// The class supports lazy loading of the referenced entity by its GUID or fully
    /// qualified name. Always use the GUID property to set the referenced entity.
    /// </remarks>
    /// <typeparam name="T">Type of the referenced entity</typeparam>
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class EntityReferenceBase<T>
        where T : Entity, new()
    {
        #region Member Variables

        private Guid guid;
        private string name;

        [NonSerialized]
        protected T value;

        #endregion
        #region Member Access Properties

        [IgnoreDataMember]
        public bool IsEmpty
        {
            get { return guid == Guid.Empty && name == null && value == null; }
        }

        /// <summary>
        /// Gets or sets the GUID of the referenced entity.
        /// </summary>
        /// <remarks>
        /// Always use this property to set the referenced entity.
        /// </remarks>
        [IgnoreDataMember]
        [XmlIgnore]
        public Guid Guid
        {
            get
            {
                if (guid == Guid.Empty && !IsEmpty)
                {
                    LoadEntity();
                }

                return guid;
            }
            set
            {
                guid = value;
                this.value = null;
            }
        }

        [DataMember]
        [XmlText]
        public string Name
        {
            get
            {
                if (name == null && !IsEmpty)
                {
                    LoadEntity();
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
                if (this.value == null && !IsEmpty)
                {
                    LoadEntity();
                }

                return this.value;
            }
            set
            {
                this.value = value;
                if (this.value != null)
                {
                    this.guid = this.value.Guid;
                    this.name = this.value.GetFullyQualifiedName();
                }
                else
                {
                    this.guid = Guid.Empty;
                    this.name = null;
                }
            }
        }

        #endregion
        #region Constructors

        public EntityReferenceBase()
        {
            InitializeMembers();
        }

        /// <summary>
        /// The copy constructor that creates a deep copy of the
        /// <b>ReferencedEntity</b> class passes as parameter.
        /// </summary>
        /// <param name="old">The original object to copy from.</param>
        public EntityReferenceBase(EntityReferenceBase<T> old)
        {
            CopyMembers(old);
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes private member variables to their default values.
        /// </summary>
        private void InitializeMembers()
        {
            this.guid = Guid.Empty;
            this.name = null;

            this.value = null;
        }

        /// <summary>
        /// Copies private member variables from another object.
        /// </summary>
        /// <remarks>
        /// Called by the copy constructor.
        /// </remarks>
        /// <param name="old">The original object to copy from.</param>
        private void CopyMembers(EntityReferenceBase<T> old)
        {
            this.guid = old.guid;
            this.name = old.name;

            this.value = old.value;
        }

        #endregion

        public abstract void LoadEntity();

        protected void LoadEntity(Context context)
        {
            if (this.value == null)
            {
                var ef = new EntityFactory(context);

                if (this.guid != Guid.Empty)
                {
                    this.value = ef.LoadEntity<T>(this.guid);
                    this.name = this.value.GetFullyQualifiedName();
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
                this.name = this.value.GetFullyQualifiedName();
                this.guid = this.value.Guid;
            }
        }

        #region IEntityReference Members

        public void ResolveNameReference()
        {
            LoadEntity();
        }

        #endregion
    }
}
