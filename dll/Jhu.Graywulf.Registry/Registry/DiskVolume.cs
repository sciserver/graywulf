/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Disk Volume</b> entity
    /// </summary>
    public partial class DiskVolume : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private DiskVolumeType diskVolumeType;
        private ExpressionProperty localPath;
        private ExpressionProperty uncPath;
        private long fullSpace;
        private long allocatedSpace;
        private long reservedSpace;
        private long speed;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the type of the disk volume according to its designation like System, Data etc.
        /// </summary>
        [DBColumn]
        public DiskVolumeType DiskVolumeType
        {
            get { return diskVolumeType; }
            set { diskVolumeType = value; }
        }

        /// <summary>
        /// Gets or sets the local path that the server itself uses for accessing the disk volume
        /// </summary>
        [XmlIgnore]
        [DBColumn(Size = 256)]
        public ExpressionProperty LocalPath
        {
            get { return localPath; }
            set { localPath = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("LocalPath")]
        public string LocalPath_ForXml
        {
            get { return localPath.Value; }
            set { localPath.Value = value; }
        }

        /// <summary>
        /// Gets or sets the UNC path that other servers can use for accessing the disk volume
        /// </summary>
        [XmlIgnore]
        [DBColumn(Size = 256)]
        public ExpressionProperty UncPath
        {
            get { return uncPath; }
            set { uncPath = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("UncPath")]
        public string UncPath_ForXml
        {
            get { return uncPath.Value; }
            set { uncPath.Value = value; }
        }

        /// <summary>
        /// Gets or sets the full space on the disk volume in bytes
        /// </summary>
        [DBColumn]
        public long FullSpace
        {
            get { return fullSpace; }
            set { fullSpace = value; }
        }

        /// <summary>
        /// Gets or sets the already allocated space on the disk volume in bytes
        /// </summary>
        [DBColumn]
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
        public long ReservedSpace
        {
            get { return reservedSpace; }
            set { reservedSpace = value; }
        }

        /// <summary>
        /// Gets or sets the maximum bandwidth information of the disk volume in bytes per second.
        /// </summary>
        [DBColumn]
        public long Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Machine</b> object to which the <b>Disk Volume</b> belongs.
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
        public DiskVolume(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DiskVolume(Machine parent)
            : base(parent.Context, parent)
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
            base.EntityType = EntityType.DiskVolume;
            base.EntityGroup = EntityGroup.Cluster;

            this.diskVolumeType = DiskVolumeType.Unknown;
            this.localPath = new ExpressionProperty(this, Constants.DiskVolumeLocalPath);
            this.uncPath = new ExpressionProperty(this, Constants.DiskVolumeUncPath);
            this.fullSpace = 0;
            this.allocatedSpace = 0;
            this.reservedSpace = 0;
            this.speed = 0;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Disk Volume</b> object to create the deep copy from.</param>
        private void CopyMembers(DiskVolume old)
        {
            this.diskVolumeType = old.diskVolumeType;
            this.localPath = new ExpressionProperty(old.localPath);
            this.uncPath = new ExpressionProperty(old.uncPath);
            this.fullSpace = old.fullSpace;
            this.allocatedSpace = old.allocatedSpace;
            this.reservedSpace = old.reservedSpace;
            this.speed = old.speed;
        }

        public override object Clone()
        {
            return new DiskVolume(this);
        }

        #endregion
    }
}
