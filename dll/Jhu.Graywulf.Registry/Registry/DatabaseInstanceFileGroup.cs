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
    /// Implements the functionality related to a database server cluster's <b>Database Instance File Group</b> entity
    /// </summary>
    public partial class DatabaseInstanceFileGroup : Entity
    {
        public enum ReferenceType : int
        {
            FileGroup = 1,
            Partition = 2,
        }

        #region Member Variables

        // --- Background storage for properties ---
        private string fileGroupName;
        private FileGroupType fileGroupType;
        private long allocatedSpace;
        private long usedSpace;
        private long reservedSpace;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.DatabaseInstanceFileGroup; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Layout; }
        }

        /// <summary>
        /// Gets or sets the value of the property refering to the file group in the physical database
        /// </summary>
        [DBColumn(Size = 50)]
        public string FileGroupName
        {
            get { return fileGroupName; }
            set { fileGroupName = value; }
        }

        [XmlIgnore]
        public FileGroup FileGroup
        {
            get { return FileGroupReference.Value; }
            set { FileGroupReference.Value = value; }
        }

        [XmlIgnore]
        public Partition Partition
        {
            get { return PartitionReference.Value; }
        }

        [DBColumn]
        public FileGroupType FileGroupType
        {
            get { return fileGroupType; }
            set { fileGroupType = value; }
        }

        /// <summary>
        /// Gets or sets the number of bytes to be allocated when the files are created.
        /// </summary>
        [DBColumn]
        [DefaultValue(0)]
        public long AllocatedSpace
        {
            get { return allocatedSpace; }
            set { allocatedSpace = value; }
        }

        /// <summary>
        /// Gets or sets the number of bytes that are filled with data in this file group.
        /// </summary>
        [DBColumn]
        [DefaultValue(0)]
        public long UsedSpace
        {
            get { return usedSpace; }
            set { usedSpace = value; }
        }

        /// <summary>
        /// Gets or sets the amount of reserved space on the disk volume in bytes.
        /// </summary>
        /// <remarks>
        /// Administrators may reserve some space on in a file group for future needs, the
        /// Cluster Management Library can send alerts when the space on in the file group is
        /// under the reserved limit.
        /// </remarks>
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
        /// Gets the <b>Database Instance</b> object to which this <b>Database Instance File Group</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public DatabaseInstance DatabaseInstance
        {
            get { return (DatabaseInstance)ParentReference.Value; }
        }

        [XmlIgnore]
        public EntityReference<FileGroup> FileGroupReference
        {
            get { return (EntityReference<FileGroup>)EntityReferences[(int)ReferenceType.FileGroup]; }
        }

        [XmlElement("FileGroup")]
        public string FileGroup_ForXml
        {
            get { return FileGroupReference.Name; }
            set { FileGroupReference.Name = value; }
        }

        [XmlIgnore]
        public EntityReference<Partition> PartitionReference
        {
            get { return (EntityReference<Partition>)EntityReferences[(int)ReferenceType.Partition]; }
        }

        [XmlElement("Partition")]
        public string Partition_ForXml
        {
            get { return PartitionReference.Name; }
            set { PartitionReference.Name = value; }
        }

        [XmlIgnore]
        public Dictionary<string, DatabaseInstanceFile> Files
        {
            get { return GetChildren<DatabaseInstanceFile>(); }
            set { SetChildren<DatabaseInstanceFile>(value); }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public DatabaseInstanceFileGroup()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Database Instance File Group</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public DatabaseInstanceFileGroup(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DatabaseInstanceFileGroup(DatabaseInstance parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Database Instance File Group</b> objects.
        /// </summary>
        /// <param name="old">The <b>Database Instance File Group</b> to copy from.</param>
        public DatabaseInstanceFileGroup(DatabaseInstanceFileGroup old)
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
            this.fileGroupName = string.Empty;
            this.fileGroupType = Registry.FileGroupType.Unknown;
            this.allocatedSpace = 0;
            this.usedSpace = 0;
            this.reservedSpace = 0;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Database Instance File Group</b> object to create the deep copy from.</param>
        private void CopyMembers(DatabaseInstanceFileGroup old)
        {
            this.fileGroupName = old.fileGroupName;
            this.fileGroupType = old.fileGroupType;
            this.allocatedSpace = old.allocatedSpace;
            this.usedSpace = old.usedSpace;
            this.reservedSpace = old.reservedSpace;
        }

        public override object Clone()
        {
            return new DatabaseInstanceFileGroup(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<FileGroup>((int)ReferenceType.FileGroup),
                new EntityReference<Partition>((int)ReferenceType.Partition),
            };
        }

        protected override EntityType[] CreateChildTypes()
        {
            return new EntityType[]
            {
                EntityType.DatabaseInstanceFile
            };
        }

        #endregion
        #region Layout Wizard Functions

        public void GenerateInstanceFiles()
        {
            GenerateInstanceFiles(null, 1.0);
        }

        public void GenerateInstanceFiles(List<DiskVolume> dataDiskVolumes, double sizeFactor)
        {
            Dictionary<string, DatabaseInstanceFile> files = new Dictionary<string, DatabaseInstanceFile>();

            List<DiskVolume> diskVolumes = new List<DiskVolume>();
            if (dataDiskVolumes == null)
            {
                this.DatabaseInstance.ServerInstance.Machine.LoadDiskVolumes(false);
                diskVolumes.AddRange(this.DatabaseInstance.ServerInstance.Machine.DiskVolumes.Values.Where<DiskVolume>(d => (d.DiskVolumeType & FileGroup.DiskVolumeType) > 0).OrderBy(i => i.Number));
            }
            else
            {
                diskVolumes.AddRange(dataDiskVolumes);
            }

            int q = 0;
            bool primary;
            int filecount = FileGroup.FileCount != 0 ? FileGroup.FileCount : diskVolumes.Count;

            for (int i = 0; i < filecount; i++)
            {
                DatabaseInstanceFile nf = new DatabaseInstanceFile(this);
                nf.DiskVolumeReference.Guid = diskVolumes[q % diskVolumes.Count].Guid;
                switch (fileGroupType)
                {
                    case FileGroupType.Data:
                        nf.DatabaseFileType = DatabaseFileType.Data;
                        break;
                    case FileGroupType.Log:
                        nf.DatabaseFileType = DatabaseFileType.Log;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                nf.LogicalName = string.Format("{0}_{1}", FileGroupName, i);
                nf.Name = nf.LogicalName;
                if (fileGroupType == FileGroupType.Log)
                {
                    primary = false;
                    nf.Filename = nf.LogicalName + ".ldf";
                }
                else if (i == 0 && StringComparer.InvariantCultureIgnoreCase.Compare(FileGroup.FileGroupName, "primary") == 0)
                {
                    primary = true;
                    nf.Filename = nf.LogicalName + ".mdf";
                }
                else
                {
                    primary = false;
                    nf.Filename = nf.LogicalName + ".ndf";
                }

                q++;

                // Set minimum file sizes
                nf.AllocatedSpace = Math.Max((long)(FileGroup.AllocatedSpace / filecount * sizeFactor), 0x80000);
                if (primary)
                {
                    nf.AllocatedSpace = Math.Max(nf.AllocatedSpace, 0x300000);
                }
                nf.Save();

                files.Add(nf.Name, nf);
            }

            this.Files = files;
        }

        #endregion
    }
}
