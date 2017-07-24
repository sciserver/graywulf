/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Database Instance File</b> entity
    /// </summary>
    public partial class DatabaseInstanceFile : Entity
    {
        public enum ReferenceType : int
        {
            DiskVolume = 1,
        }

        #region Member Variables

        // --- Background storage for properties ---
        private DatabaseFileType databaseFileType;
        private string logicalName;
        private string filename;
        private long allocatedSpace;
        private long usedSpace;
        private long reservedSpace;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.DatabaseInstanceFile; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Layout; }
        }

        /// <summary>
        /// Gets or sets the value determining the type of this file: Log or Data
        /// </summary>
        [DBColumn]
        public DatabaseFileType DatabaseFileType
        {
            get { return databaseFileType; }
            set { databaseFileType = value; }
        }

        /// <summary>
        /// Gets or sets the logical name of a file
        /// </summary>
        [DBColumn(Size = 50)]
        public string LogicalName
        {
            get { return logicalName; }
            set { logicalName = value; }
        }

        /// <summary>
        /// Gets or sets the file name of the file
        /// </summary>
        [DBColumn(Size = 256)]
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary>
        /// Gets or sets the size of the file in bytes.
        /// </summary>
        [DBColumn]
        [DefaultValue(0)]
        public long AllocatedSpace
        {
            get { return allocatedSpace; }
            set { allocatedSpace = value; }
        }

        /// <summary>
        /// Gets or sets the number of bytes in the file that is used for storing data.
        /// </summary>
        [DBColumn]
        [DefaultValue(0)]
        public long UsedSpace
        {
            get { return usedSpace; }
            set { usedSpace = value; }
        }

        /// <summary>
        /// Gets or sets the amount of reserver space in the file in bytes.
        /// </summary>
        [DBColumn]
        [DefaultValue(0)]
        public long ReservedSpace
        {
            get { return reservedSpace; }
            set { reservedSpace = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the file group associated with the database file.
        /// </summary>
        /// <remarks>
        /// Returns null if the file does not belong directly to a file group
        /// but to a database instance, i.e. the file is a log file.
        /// </remarks>
        public DatabaseInstanceFileGroup DatabaseInstanceFileGroup
        {
            get { return Parent as DatabaseInstanceFileGroup; }
        }

        /// <summary>
        /// Gets the reference object to the disk volume associated with the database file.
        /// </summary>
        [XmlIgnore]
        public EntityReference<DiskVolume> DiskVolumeReference
        {
            get { return (EntityReference<DiskVolume>)EntityReferences[(int)ReferenceType.DiskVolume]; }
        }

        /// <summary>
        /// Gets the disk volume associated with the database file.
        /// </summary>
        [XmlIgnore]
        public DiskVolume DiskVolume
        {
            get { return DiskVolumeReference.Value; }
            set { DiskVolumeReference.Value = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("DiskVolume")]
        public string DiskVolume_ForXml
        {
            get { return DiskVolumeReference.Name; }
            set { DiskVolumeReference.Name = value; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public DatabaseInstanceFile()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Database Instance File</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public DatabaseInstanceFile(RegistryContext context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DatabaseInstanceFile(DatabaseInstance parent)
            : base(parent.RegistryContext, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DatabaseInstanceFile(DatabaseInstanceFileGroup parent)
            : base(parent.RegistryContext, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Database Instance File</b> objects.
        /// </summary>
        /// <param name="old">The <b>Database Instance File</b> to copy from.</param>
        public DatabaseInstanceFile(DatabaseInstanceFile old)
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
            this.databaseFileType = DatabaseFileType.Unknown;
            this.logicalName = string.Empty;
            this.filename = string.Empty;
            this.allocatedSpace = 0;
            this.usedSpace = 0;
            this.reservedSpace = 0;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Database Instance File</b> object to create the deep copy from.</param>
        private void CopyMembers(DatabaseInstanceFile old)
        {
            this.databaseFileType = old.databaseFileType;
            this.logicalName = old.logicalName;
            this.filename = old.filename;
            this.allocatedSpace = old.allocatedSpace;
            this.usedSpace = old.usedSpace;
            this.reservedSpace = old.reservedSpace;
        }

        internal override bool CompareMembers(Entity other)
        {
            bool eq = base.CompareMembers(other);
            var o = other as DatabaseInstanceFile;

            eq &= this.databaseFileType == o.databaseFileType;
            eq &= this.logicalName == o.logicalName;
            eq &= this.filename == o.filename;
            eq &= this.allocatedSpace == o.allocatedSpace;
            eq &= this.usedSpace == o.usedSpace;
            eq &= this.reservedSpace == o.reservedSpace;

            return eq;
        }

        internal override void UpdateMembers(Entity other)
        {
            base.UpdateMembers(other);
            var o = other as DatabaseInstanceFile;
            CopyMembers(o);
        }

        public override object Clone()
        {
            return new DatabaseInstanceFile(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<DiskVolume>((int)ReferenceType.DiskVolume)
            };
        }

        #endregion


        /// <summary>
        /// Returns the full path of the database file as seen by the local machine.
        /// </summary>
        /// <returns>The full local path to the database file.</returns>
        public string GetFullLocalFilename()
        {
            return GetFilenameWithPath(DiskVolume.LocalPath.ResolvedValue);
        }

        /// <summary>
        /// Returns the full path of the database file as seen on the network.
        /// </summary>
        /// <returns>The full network path to the database file.</returns>
        public string GetFullUncFilename()
        {
            return GetFilenameWithPath(DiskVolume.UncPath.ResolvedValue);
        }

        /// <summary>
        /// Generates the file path from the cluster schema hierarcy.
        /// </summary>
        /// <param name="basePath">Root path of the disk volume.</param>
        /// <returns>The full path to the database file.</returns>
        private string GetFilenameWithPath(string basePath)
        {
            var di = DatabaseInstanceFileGroup.DatabaseInstance;

            var filename = Path.Combine(
                basePath,
                di.DatabaseDefinition.Parent.Name,
                di.DatabaseDefinition.Name,
                di.Slice.Name,
                di.DatabaseVersion.Name);

            switch (di.DatabaseDefinition.LayoutType)
            {
                case DatabaseLayoutType.Sliced:
                    // Use database instance name
                    filename = Path.Combine(
                        filename,
                        di.Name,
                        this.Filename);
                    break;
                case DatabaseLayoutType.Monolithic:
                case DatabaseLayoutType.Mirrored:
                    // Use database name
                    filename = Path.Combine(
                        filename,
                        di.DatabaseName,
                        this.Filename);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return filename;
        }
    }
}
