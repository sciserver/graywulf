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
    public partial class DiskGroup : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private DiskGroupType type;
        private long fullSpace;
        private long allocatedSpace;
        private long reservedSpace;
        private long readBandwidth;
        private long writeBandwidth;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.DiskGroup; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Cluster; }
        }

        /// <summary>
        /// Gets or sets the type of the disk volume according to its designation like System, Data etc.
        /// </summary>
        [DBColumn]
        public DiskGroupType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the full space on the disk volume in bytes
        /// </summary>
        [DBColumn]
        [DefaultValue(0)]
        public long FullSpace
        {
            get { return fullSpace; }
            set { fullSpace = value; }
        }

        /// <summary>
        /// Gets or sets the already allocated space on the disk volume in bytes
        /// </summary>
        [DBColumn]
        [DefaultValue(0)]
        public long AllocatedSpace
        {
            get { return allocatedSpace; }
            set { allocatedSpace = value; }
        }

        /// <summary>
        /// Gets or sets the amount of reserved space on the disk volume in bytes.
        /// </summary>
        /// <remarks>
        /// Administrators may reserve some space on a disk volume for future needs, the
        /// Cluster Management Library can send alerts when the space on the volume is
        /// under the reserved limit.
        /// </remarks>
        [DBColumn]
        [DefaultValue(0)]
        public long ReservedSpace
        {
            get { return reservedSpace; }
            set { reservedSpace = value; }
        }

        /// <summary>
        /// Gets or sets the maximum bandwidth information of the disk volume in bytes per second.
        /// </summary>
        [DBColumn]
        [DefaultValue(0)]
        public long ReadBandwidth
        {
            get { return readBandwidth; }
            set { readBandwidth = value; }
        }

        /// <summary>
        /// Gets or sets the maximum bandwidth information of the disk volume in bytes per second.
        /// </summary>
        [DBColumn]
        [DefaultValue(0)]
        public long WriteBandwidth
        {
            get { return writeBandwidth; }
            set { writeBandwidth = value; }
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
        public Machine Machine
        {
            get { return (Machine)ParentReference.Value; }
        }

        [XmlIgnore]
        public Dictionary<string, DiskVolume> DiskVolumes
        {
            get { return GetChildren<DiskVolume>(); }
            set { SetChildren<DiskVolume>(value); }
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
        public DiskGroup()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Disk Volume</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public DiskGroup(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DiskGroup(Machine parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Disk Volume</b> objects.
        /// </summary>
        /// <param name="old">The <b>Disk Volume</b> to copy from.</param>
        public DiskGroup(DiskGroup old)
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
            this.type = DiskGroupType.Unknown;
            this.fullSpace = 0;
            this.allocatedSpace = 0;
            this.reservedSpace = 0;
            this.readBandwidth = 0;
            this.writeBandwidth = 0;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Disk Volume</b> object to create the deep copy from.</param>
        private void CopyMembers(DiskGroup old)
        {
            this.type = old.type;
            this.fullSpace = old.fullSpace;
            this.allocatedSpace = old.allocatedSpace;
            this.reservedSpace = old.reservedSpace;
            this.readBandwidth = old.readBandwidth;
            this.writeBandwidth = old.writeBandwidth;
        }

        public override object Clone()
        {
            return new DiskGroup(this);
        }

        protected override EntityType[] CreateChildTypes()
        {
            return new EntityType[]
            {
                EntityType.DiskVolume,
            };
        }

        #endregion
    }
}
