using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Registry
{
    public class UserIdentity : Entity
    {

        #region Member variables

        private string protocol;
        private string authority;
        private string identifier;

        #endregion
        #region Member access properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.UserIdentity; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Domain; }
        }

        [DBColumn(Size = 25)]
        [DefaultValue("")]
        [XmlElement]
        public string Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }

        [DBColumn(Size = 250)]
        [DefaultValue("")]
        [XmlElement]
        public string Authority
        {
            get { return authority; }
            set { authority = value; }
        }

        [DBColumn(Size = 250)]
        [DefaultValue("")]
        [XmlElement]
        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }

        #endregion
        #region Navigation Properties

        [XmlIgnore]
        public User User
        {
            get { return (User)Parent; }
        }

        #endregion
        #region Constructors and initializer function

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public UserIdentity()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>User</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public UserIdentity(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public UserIdentity(User parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>User</b> objects.
        /// </summary>
        /// <param name="old">The <b>User</b> to copy from.</param>
        public UserIdentity(UserIdentity old)
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
            this.protocol = String.Empty;
            this.authority = String.Empty;
            this.identifier = String.Empty;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>User</b> object to create the deep copy from.</param>
        private void CopyMembers(UserIdentity old)
        {
            this.protocol = old.protocol;
            this.authority = old.authority;
            this.identifier = old.identifier;
        }

        public override object Clone()
        {
            return new UserIdentity(this);
        }

        #endregion
    }
}
