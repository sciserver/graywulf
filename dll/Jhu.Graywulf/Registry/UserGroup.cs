/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>User Group</b> entity
    /// </summary>
    /// <remarks>
    /// It is a a very simple preliminary implementation without the entity level user access system.
    /// </remarks>
    public partial class UserGroup : Entity
    {
        #region Member Variables

        #endregion
        #region Member Access Properties
        #endregion
        #region Navigation Properties

        public Cluster Cluster
        {
            get { return (Cluster)Parent; }
        }

        public Domain Domain
        {
            get { return (Domain)Parent; }
        }

        #endregion
        #region Validation Properties
        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public UserGroup()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>User Group</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public UserGroup(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>User Group</b> object and setting the
        /// context and the parent (Cluster).
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity of the <b>User Group</b> of type Cluster.</param>
        public UserGroup(Context context, Cluster parent)
            : base(context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>User Group</b> object and setting the
        /// context and the parent (Domain).
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity of the <b>User Group</b> of type Domain.</param>
        public UserGroup(Context context, Domain parent)
            : base(context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>User Group</b> objects.
        /// </summary>
        /// <param name="old">The <b>User Group</b> to copy from.</param>
        public UserGroup(UserGroup old)
            : base(old)
        {
            CopyMembers(old);
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes member variables to their initial values.
        /// </summary>
        /// <remarks>
        /// This function is called by the contructors.
        /// </remarks>
        private void InitializeMembers()
        {
            base.EntityType = EntityType.UserGroup;
            base.EntityGroup = EntityGroup.Security;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>User Group</b> object to create the deep copy from.</param>
        private void CopyMembers(UserGroup old)
        {
        }

        #endregion
    }
}
