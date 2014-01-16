/* Copyright */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Cluster</b> entity
    /// </summary>
    public partial class Domain : Entity
    {
        public enum ReferenceType : int
        {
            StandardUserGroup = 1,
        }

        #region Member Variables

        // --- Background storage for properties ---
        private string shortTitle;
        private string longTitle;
        private string email;
        private string copyright;
        private string disclaimer;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.Domain; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Domain; }
        }

        [DBColumn(Size = 50)]
        public string ShortTitle
        {
            get { return shortTitle; }
            set { shortTitle = value; }
        }

        [DBColumn(Size = 256)]
        public string LongTitle
        {
            get { return longTitle; }
            set { longTitle = value; }
        }

        [DBColumn(Size = 128)]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [DBColumn(Size = 1024)]
        public string Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }

        [DBColumn(Size = 1024)]
        public string Disclaimer
        {
            get { return disclaimer; }
            set { disclaimer = value; }
        }

        [XmlIgnore]
        public UserGroup StandardUserGroup
        {
            get { return StandardUserGroupReference.Value; }
            set { StandardUserGroupReference.Value = value; }
        }

        #endregion
        #region Validation Properties
        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Cluster</b> object to which this <b>Domain</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public Cluster Cluster
        {
            get { return (Cluster)ParentReference.Value; }
        }

        [XmlIgnore]
        public EntityReference<UserGroup> StandardUserGroupReference
        {
            get { return (EntityReference<UserGroup>)EntityReferences[(int)ReferenceType.StandardUserGroup]; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("StandardUserGroup")]
        public string StandardUserGroup_ForXml
        {
            get { return StandardUserGroupReference.Name; }
            set { StandardUserGroupReference.Name = value; }
        }

        [XmlIgnore]
        public Dictionary<string, Federation> Federations
        {
            get { return GetChildren<Federation>(); }
            set { SetChildren<Federation>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, User> Users
        {
            get { return GetChildren<User>(); }
            set { SetChildren<User>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, UserGroup> UserGroups
        {
            get { return GetChildren<UserGroup>(); }
            set { SetChildren<UserGroup>(value); }
        }

        #endregion
        #region Constructors and initializer functions

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public Domain()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Domamin</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public Domain(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        public Domain(Cluster parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Domain</b> objects.
        /// </summary>
        /// <param name="old">The <b>Domain</b> to copy from.</param>
        public Domain(Domain old)
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
            this.shortTitle = String.Empty;
            this.longTitle = String.Empty;
            this.email = String.Empty;
            this.copyright = String.Empty;
            this.disclaimer = String.Empty;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Domain</b> object to create the deep copy from.</param>
        private void CopyMembers(Domain old)
        {
            this.shortTitle = old.shortTitle;
            this.longTitle = old.longTitle;
            this.email = old.email;
            this.copyright = old.copyright;
            this.disclaimer = old.disclaimer;
        }

        public override object Clone()
        {
            return new Domain(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<UserGroup>((int)ReferenceType.StandardUserGroup),
            };
        }

        protected override EntityType[] CreateChildTypes()
        {
            return new EntityType[]
            {
                EntityType.Federation,
                EntityType.User,
                EntityType.UserGroup
            };
        }

        #endregion


    }
}
