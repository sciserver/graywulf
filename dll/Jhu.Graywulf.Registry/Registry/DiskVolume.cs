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
    /// Implements the functionality related to a database server cluster's <b>Disk Volume</b> entity
    /// </summary>
    public partial class DiskVolume : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private DiskVolumeType type;
        private ExpressionProperty localPath;
        private ExpressionProperty uncPath;
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
            get { return EntityType.DiskVolume; }
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
        public DiskVolumeType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the local path that the server itself uses for accessing the disk volume
        /// </summary>
        [DBColumn(Size = 256)]
        public ExpressionProperty LocalPath
        {
            get { return localPath; }
            set { localPath = value; }
        }

        /// <summary>
        /// Gets or sets the UNC path that other servers can use for accessing the disk volume
        /// </summary>
        [DBColumn(Size = 256)]
        public ExpressionProperty UncPath
        {
            get { return uncPath; }
            set { uncPath = value; }
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
        /// Gets the <b>Disk Group</b> object to which the <b>Disk Volume</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public DiskGroup DiskGroup
        {
            get { return (DiskGroup)ParentReference.Value; }
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
        public DiskVolume()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Disk Volume</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public DiskVolume(RegistryContext context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DiskVolume(DiskGroup parent)
            : base(parent.RegistryContext, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Disk Volume</b> objects.
        /// </summary>
        /// <param name="old">The <b>Disk Volume</b> to copy from.</param>
        public DiskVolume(DiskVolume old)
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
            this.type = DiskVolumeType.Unknown;
            this.localPath = new ExpressionProperty(this, Constants.DiskVolumeLocalPath);
            this.uncPath = new ExpressionProperty(this, Constants.DiskVolumeUncPath);
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
        private void CopyMembers(DiskVolume old)
        {
            this.type = old.type;
            this.localPath = new ExpressionProperty(old.localPath);
            this.uncPath = new ExpressionProperty(old.uncPath);
            this.fullSpace = old.fullSpace;
            this.allocatedSpace = old.allocatedSpace;
            this.reservedSpace = old.reservedSpace;
            this.readBandwidth = old.readBandwidth;
            this.writeBandwidth = old.writeBandwidth;
        }

        internal override bool CompareMembers(Entity other)
        {
            bool eq = base.CompareMembers(other);
            var o = other as DiskVolume;

            eq &= this.type == o.type;
            eq &= this.localPath.CompareMembers(o.localPath);
            eq &= this.uncPath.CompareMembers(o.uncPath);
            eq &= this.fullSpace == o.fullSpace;
            eq &= this.allocatedSpace == o.allocatedSpace;
            eq &= this.reservedSpace == o.reservedSpace;
            eq &= this.readBandwidth == o.readBandwidth;
            eq &= this.writeBandwidth == o.writeBandwidth;

            return eq;
        }

        internal override void UpdateMembers(Entity other)
        {
            base.UpdateMembers(other);
            var o = other as DiskVolume;
            CopyMembers(o);
        }

        public override object Clone()
        {
            return new DiskVolume(this);
        }

        #endregion
    }
}
