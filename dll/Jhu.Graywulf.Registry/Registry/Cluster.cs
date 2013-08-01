/* Copyright */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Cluster</b> entity
    /// </summary>
    public partial class Cluster : Entity
    {
        public static class AppSettings
        {
            private static string GetValue(string key)
            {
                return (string)((NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Registry/Cluster"))[key];
            }

            public static string ClusterName
            {
                get { return GetValue("ClusterName"); }
            }
        }

        #region Validation Properties
        #endregion
        #region Navigation Properties

        [XmlIgnore]
        public Dictionary<string, MachineRole> MachineRoles
        {
            get { return GetChildren<MachineRole>(); }
            set { SetChildren<MachineRole>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, Domain> Domains
        {
            get { return GetChildren<Domain>(); }
            set { SetChildren<Domain>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, DatabaseDefinition> DatabaseDefinitions
        {
            get { return GetChildren<DatabaseDefinition>(); }
            set { SetChildren<DatabaseDefinition>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, QueueDefinition> QueueDefinitions
        {
            get { return GetChildren<QueueDefinition>(); }
            set { SetChildren<QueueDefinition>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, JobDefinition> JobDefinitions
        {
            get { return GetChildren<JobDefinition>(); }
            set { SetChildren<JobDefinition>(value); }
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
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public Cluster()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Cluster</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public Cluster(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Cluster</b> objects.
        /// </summary>
        /// <param name="old">The <b>Cluster</b> to copy from.</param>
        public Cluster(Cluster old)
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
            base.EntityType = EntityType.Cluster;
            base.EntityGroup = EntityGroup.All;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Cluster</b> object to create the deep copy from.</param>
        private void CopyMembers(Cluster old)
        {
        }

        public override object Clone()
        {
            return new Cluster(this);
        }

        protected override Type[] CreateChildTypes()
        {
            return new Type[] {
                    typeof(MachineRole),
                    typeof(Domain),
                    typeof(DatabaseDefinition),
                    typeof(QueueDefinition),
                    typeof(JobDefinition),
                    typeof(User),
                    typeof(UserGroup)
            };
        }

        #endregion
    }
}
