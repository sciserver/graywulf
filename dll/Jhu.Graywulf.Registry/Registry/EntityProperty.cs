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
    public class EntityProperty<T> : EntityReferenceBase<T>
        where T : Entity, new()
    {
        #region Member Variables

        [NonSerialized]
        private Context context;

        #endregion
        #region Member Access Properties

        [IgnoreDataMember]
        [XmlIgnore]
        public Context Context
        {
            get { return context; }
            set
            {
                context = value;

                if (this.value != null)
                {
                    this.value.Context = value;
                }
            }
        }

        #endregion
        #region Constructors

        public EntityProperty()
        {
            InitializeMembers();
        }

        public EntityProperty(Context context)
        {
            InitializeMembers();

            this.context = context;
        }

        /// <summary>
        /// The copy constructor that creates a deep copy of the
        /// <b>ReferencedEntity</b> class passes as parameter.
        /// </summary>
        /// <param name="old">The original object to copy from.</param>
        public EntityProperty(EntityProperty<T> old)
            :base(old)
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
            this.context = null;
        }

        /// <summary>
        /// Copies private member variables from another object.
        /// </summary>
        /// <remarks>
        /// Called by the copy constructor.
        /// </remarks>
        /// <param name="old">The original object to copy from.</param>
        private void CopyMembers(EntityProperty<T> old)
        {
            this.context = old.context;
        }

        #endregion

        public override void LoadEntity()
        {
            LoadEntity(context);
        }
    }
}
