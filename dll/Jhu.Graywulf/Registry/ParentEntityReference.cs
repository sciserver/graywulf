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
    public class ParentEntityReference<T> : EntityReferenceBase<T>
        where T : Entity, new()
    {
        #region Member Variables
        
        [NonSerialized]
        private Entity parentEntity;

        #endregion
        #region Constructors

        /// <summary>
        /// The constructor sets the referenced entity to the one passed as parameter.
        /// </summary>
        /// <param name="referencingEntity">Entity to reference</param>
        public ParentEntityReference(Entity parentEntity)
            :base()
        {
            InitializeMembers();

            this.parentEntity = parentEntity;
        }

        /// <summary>
        /// The copy constructor that creates a deep copy of the
        /// <b>ReferencedEntity</b> class passes as parameter.
        /// </summary>
        /// <param name="old">The original object to copy from.</param>
        public ParentEntityReference(ParentEntityReference<T> old)
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
            this.parentEntity = null;
        }

        /// <summary>
        /// Copies private member variables from another object.
        /// </summary>
        /// <remarks>
        /// Called by the copy constructor.
        /// </remarks>
        /// <param name="old">The original object to copy from.</param>
        private void CopyMembers(ParentEntityReference<T> old)
        {
            this.parentEntity = old.parentEntity;
        }

        #endregion

        public override void LoadEntity()
        {
            LoadEntity(parentEntity.Context);
        }
    }
}
