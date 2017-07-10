/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>User Role</b> entity
    /// </summary>
    /// <remarks>
    /// It is a a very simple preliminary implementation without the entity level user access system.
    /// </remarks>
    public partial class UserRole : Entity
    {
        #region Member Variables

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.UserRole; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Domain; }
        }

        #endregion
        #region Navigation Properties

        [XmlIgnore]
        public Domain Domain
        {
            get { return (Domain)ParentReference.Value; }
        }

        #endregion
        #region Validation Properties
        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public UserRole()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>User Role</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public UserRole(RegistryContext context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>User Role</b> object and setting the
        /// context and the parent (Domain).
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity of the <b>User Role</b> of type Domain.</param>
        public UserRole(Domain parent)
            : base(parent.RegistryContext, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>User Role</b> objects.
        /// </summary>
        /// <param name="old">The <b>User Role</b> to copy from.</param>
        public UserRole(UserRole old)
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
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>User Role</b> object to create the deep copy from.</param>
        private void CopyMembers(UserRole old)
        {
        }

        public override object Clone()
        {
            return new UserRole();
        }

        #endregion
    }
}
