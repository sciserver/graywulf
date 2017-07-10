/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a user to a user group.
    /// </summary>
    public partial class UserGroupMembership : Entity
    {
        public enum ReferenceType : int
        {
            UserGroup = 1,
            UserRole = 2
        }

        #region Member Variables


        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.UserGroupMembership; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Domain; }
        }

        [XmlIgnore]
        public UserGroup UserGroup
        {
            get { return UserGroupReference.Value; }
            set { UserGroupReference.Value = value; }
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
        public EntityReference<UserGroup> UserGroupReference
        {
            get { return (EntityReference<UserGroup>)EntityReferences[(int)ReferenceType.UserGroup]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("UserGroup")]
        public string UserGroup_ForXml
        {
            get { return UserGroupReference.Name; }
            set { UserGroupReference.Name = value; }
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
        public UserGroupMembership()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Slice</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public UserGroupMembership(RegistryContext context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public UserGroupMembership(User parent)
            : base(parent.RegistryContext, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Slice</b> objects.
        /// </summary>
        /// <param name="old">The <b>Slice</b> to copy from.</param>
        public UserGroupMembership(UserGroupMembership old)
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
        private void CopyMembers(UserGroupMembership old)
        {
        }

        public override object Clone()
        {
            return new UserGroupMembership(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<UserGroup>((int)ReferenceType.UserGroup),
                new EntityReference<UserRole>((int)ReferenceType.UserRole),
            };
        }

        #endregion
    }
}
