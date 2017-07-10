/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Disk Group</b> entity
    /// </summary>
    /// <remarks>
    /// Disk group is a collection of disk volumes, typically a JBOD
    /// </remarks>
    public partial class ServerInstanceDiskGroup : Entity
    {
        public enum ReferenceType : int
        {
            DiskGroup = 1,
        }

        #region Member Variables

        // --- Background storage for properties ---
        private DiskDesignation diskDesignation;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.ServerInstanceDiskGroup; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Cluster; }
        }

        [XmlIgnore]
        public DiskGroup DiskGroup
        {
            get { return DiskGroupReference.Value; }
            set { DiskGroupReference.Value = value; }
        }

        /// <summary>
        /// Gets or sets the type of the disk volume according to its designation like System, Data etc.
        /// </summary>
        [DBColumn]
        public DiskDesignation DiskDesignation
        {
            get { return diskDesignation; }
            set { diskDesignation = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>ServerInstance</b> object which the <b>Disk Group</b> belongs to.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public ServerInstance ServerInstance
        {
            get { return (ServerInstance)ParentReference.Value; }
        }

        [XmlIgnore]
        public EntityReference<DiskGroup> DiskGroupReference
        {
            get { return (EntityReference<DiskGroup>)EntityReferences[(int)ReferenceType.DiskGroup]; }
        }

        [XmlElement("DiskGroup")]
        public string DiskGroup_ForXml
        {
            get { return DiskGroupReference.Name; }
            set { DiskGroupReference.Name = value; }
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
        public ServerInstanceDiskGroup()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Server Instance Disk Group</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public ServerInstanceDiskGroup(RegistryContext context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public ServerInstanceDiskGroup(ServerInstance parent)
            : base(parent.RegistryContext, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Disk Volume</b> objects.
        /// </summary>
        /// <param name="old">The <b>Disk Volume</b> to copy from.</param>
        public ServerInstanceDiskGroup(ServerInstanceDiskGroup old)
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
            this.diskDesignation = DiskDesignation.Unknown;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Disk Volume</b> object to create the deep copy from.</param>
        private void CopyMembers(ServerInstanceDiskGroup old)
        {
            this.diskDesignation = old.diskDesignation;
        }

        internal override bool CompareMembers(Entity other)
        {
            bool eq = base.CompareMembers(other);
            var o = other as ServerInstanceDiskGroup;

            eq &= this.diskDesignation == o.diskDesignation;

            return eq;
        }

        internal override void UpdateMembers(Entity other)
        {
            base.UpdateMembers(other);
            var o = other as ServerInstanceDiskGroup;
            CopyMembers(o);
        }

        public override object Clone()
        {
            return new ServerInstanceDiskGroup(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<DiskGroup>((int)ReferenceType.DiskGroup),
            };
        }

        #endregion
    }
}
