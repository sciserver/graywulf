/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class EntityReference<T> : EntityReferenceBase<T>, IEntityReference, ICloneable
        where T : Entity, new()
    {
        #region Member Variables

        [NonSerialized]
        private Entity referencingEntity;

        private int referenceType;

        #endregion
        #region Member Access Properties

        Entity IEntityReference.ReferencingEntity
        {
            get { return referencingEntity; }
            set { referencingEntity = value; }
        }

        int IEntityReference.ReferenceType
        {
            get { return referenceType; }
        }

        Guid IEntityReference.ReferencedEntityGuid
        {
            get { return Guid; }
            set { Guid = value; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// The constructor sets the referenced entity to the one passed as parameter.
        /// </summary>
        /// <param name="referencingEntity">Entity to reference</param>
        public EntityReference(int referenceType)
            : base()
        {
            InitializeMembers();

            this.referenceType = referenceType;
        }

        /// <summary>
        /// The copy constructor that creates a deep copy of the
        /// <b>ReferencedEntity</b> class passes as parameter.
        /// </summary>
        /// <param name="old">The original object to copy from.</param>
        public EntityReference(EntityReference<T> old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes private member variables to their default values.
        /// </summary>
        private void InitializeMembers()
        {
            this.referencingEntity = null;
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
            this.referencingEntity = old.referencingEntity;
            this.referenceType = old.referenceType;
        }

        public object Clone()
        {
            return new EntityReference<T>(this);
        }

        IEntityReference IEntityReference.Clone()
        {
            return new EntityReference<T>(this);
        }

        #endregion

        public override void LoadEntity()
        {
            LoadEntity(referencingEntity.Context);
        }
    }
}
