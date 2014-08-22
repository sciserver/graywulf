/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a user to a user role.
    /// </summary>
    public partial class UserRoleMembership : Entity
    {
        public enum ReferenceType : int
        {
            UserRole = 1
        }

        #region Member Variables


        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.UserRoleMembership; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Domain; }
        }

        [XmlIgnore]
        public UserRole UserRole
        {
            get { return UserRoleReference.Value; }
            set { UserRoleReference.Value = value; }
        }

        #endregion
        #region Navigation Properties

        [XmlIgnore]
        public User User
        {
            get
            {
                return (User)ParentReference.Value;
            }
        }

        [XmlIgnore]
        public EntityReference<UserRole> UserRoleReference
        {
            get { return (EntityReference<UserRole>)EntityReferences[(int)ReferenceType.UserRole]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("UserRole")]
        public string UserRole_ForXml
        {
            get { return UserRoleReference.Name; }
            set { UserRoleReference.Name = value; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public UserRoleMembership()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Slice</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public UserRoleMembership(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public UserRoleMembership(User parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Slice</b> objects.
        /// </summary>
        /// <param name="old">The <b>Slice</b> to copy from.</param>
        public UserRoleMembership(UserRoleMembership old)
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
        /// <param name="old">A <b>Slice</b> object to create the deep copy from.</param>
        private void CopyMembers(UserRoleMembership old)
        {
        }

        public override object Clone()
        {
            return new UserRoleMembership(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<UserRole>((int)ReferenceType.UserRole),
            };
        }

        #endregion
    }
}
