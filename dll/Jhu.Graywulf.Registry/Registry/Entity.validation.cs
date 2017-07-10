using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    public partial class Entity : ContextObject
    {
        #region Validation Properties

        /// <summary>
        /// Returns <b>true</b> if the entity is locked by another process.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="ReleaseLock"/> function to release an obtained lock on
        /// the property.
        /// </remarks>
        public bool IsLocked
        {
            get
            {
                return (lockOwner != Guid.Empty && lockOwner != RegistryContext.ContextGuid);
            }
        }

        /// <summary>
        /// Returns <b>true</b> if the entity can be modified.
        /// </summary>
        /// <remarks>
        /// The <b>false</b> value of this property doesn't prevents the class to persist itself to the database,
        /// this property is used by the Cluster Management Console to disable the Edit button on the forms
        /// when modifying the entity by the administrators would lead to inconsistency.
        /// </remarks>
        public bool CanModify()
        {
            return IsLocked;
        }

        /// <summary>
        /// Returns <b>true</b> if the entity can be deleted.
        /// </summary>
        /// <remarks>
        /// TODO Add remarks when delete code is done.
        /// </remarks>
        public bool CanDelete(bool checkRecursive, bool checkReferencing)
        {
            // Do entity verifications
            if (system) return false;
            if (deleted) return false;
            if (LockOwner != Guid.Empty) return false;

            // First make sure it's not deployed or transient
            // Only new and undeployed entities can be deleted
            if ((deploymentState & (DeploymentState.New | DeploymentState.Undeployed)) == 0)
            {
                return false;
            }

            if (checkRecursive)
            {
                // Make sure all children can be deleted
                LoadAllChildren(true);
                foreach (Entity e in EnumerateAllChildren())
                {
                    if (!e.CanDelete(checkRecursive, checkReferencing))
                    {
                        return false;
                    }
                }
            }

            if (checkReferencing)
            {
                // Check if there are any referencing entities
                var ef = new EntityFactory(RegistryContext);
                if (ef.FindReferencing(this).Count() > 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns <b>true</b> if the entity can be deployed.
        /// </summary>
        public virtual bool CanDeploy(bool checkRecursive)
        {
            // Only new and undeployed entities can be deployed
            if ((deploymentState & (DeploymentState.New | DeploymentState.Undeployed)) == 0)
            {
                return false;
            }

            if (checkRecursive)
            {
                // The generic behaviour is that an entity can be deployed only if it's parent is
                // already deployed. Other behaviour is processed in the derived classes.
                if (Parent.DeploymentState != DeploymentState.Deployed)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns <b>true</b> if the entity can be undeployed.
        /// </summary>
        public virtual bool CanUndeploy(bool checkRecursive)
        {
            // Only deployed entities can be undeployed
            if (deploymentState != DeploymentState.Deployed)
            {
                return false;
            }

            if (checkRecursive)
            {
                // Only entities with undeployed children can be undeployed
                LoadAllChildren(true);
                foreach (Entity e in EnumerateAllChildren())
                {
                    if (!e.CanUndeploy(checkRecursive))
                    {
                        return false;
                    }
                }
            }

            // TODO: check referencing? 

            return true;
        }

        #endregion
        #region Validation Functions

        /// <summary>
        /// Validates the entity and returns results of the validation.
        /// </summary>
        /// <param name="messages">Returns an array of validation messages.</param>
        /// <param name="recursive">If true, validation is done on the child entities too.</param>
        /// <param name="deployment">If true, validates against deployed objects otherwise checks
        /// the cluster schema consistency only.</param>
        /// <returns>True if the validation was successful and the entity configuration is valid.</returns>
        protected virtual bool Validate(out ValidationMessage[] messages, bool recursive, bool deployment)
        {
            throw new NotImplementedException();
            // TODO to be implemented
            //messages = null;
            //return true;
        }

        #endregion
    }
}
