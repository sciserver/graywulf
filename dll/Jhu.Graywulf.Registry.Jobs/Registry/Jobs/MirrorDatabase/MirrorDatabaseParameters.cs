using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry.Jobs.MirrorDatabase
{
    /// <summary>
    /// Represents a set of parameters that describe a database mirroring job.
    /// </summary>
    [Serializable]
    [DataContract(Name = "MirrorDatabaseParameters", Namespace = "")]
    public class MirrorDatabaseParameters
    {
        #region Private member variables

        private Guid[] sourceDatabaseInstanceGuids;
        private Guid[] destinationDatabaseInstanceGuids;
        private bool cascadedCopy;
        private bool skipExistingFiles;
        private bool attachAsReadOnly;
        private bool runCheckDb;

        #endregion
        #region Properties

        [DataMember]
        public Guid[] SourceDatabaseInstanceGuids
        {
            get { return sourceDatabaseInstanceGuids; }
            set { sourceDatabaseInstanceGuids = value; }
        }

        [DataMember]
        public Guid[] DestinationDatabaseInstanceGuids
        {
            get { return destinationDatabaseInstanceGuids; }
            set { destinationDatabaseInstanceGuids = value; }
        }

        [DataMember]
        public bool CascadedCopy
        {
            get { return cascadedCopy; }
            set { cascadedCopy = value; }
        }

        [DataMember]
        public bool SkipExistingFiles
        {
            get { return skipExistingFiles; }
            set { skipExistingFiles = value; }
        }

        [DataMember]
        public bool AttachAsReadOnly
        {
            get { return attachAsReadOnly; }
            set { attachAsReadOnly = value; }
        }

        [DataMember]
        public bool RunCheckDb
        {
            get { return runCheckDb; }
            set { runCheckDb = value; }
        }

        #endregion
        #region Constructors and initializers

        
        public MirrorDatabaseParameters()
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.sourceDatabaseInstanceGuids = null;
            this.destinationDatabaseInstanceGuids = null;
            this.cascadedCopy = true;
            this.skipExistingFiles = true;
            this.attachAsReadOnly = true;
            this.runCheckDb = false;
        }

        #endregion
    }
}
