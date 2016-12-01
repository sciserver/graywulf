using System;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    [Serializable]
    public class CopyTablesParameters
    {
        #region Private member variables

        private CopyTablesItem[] items;
        private int timeout;

        #endregion
        #region Properties

        [DataMember]
        public CopyTablesItem[] Items
        {
            get { return items; }
            set { items = value; }
        }

        /// <summary>
        /// Gets or sets the execution time-out.
        /// </summary>
        [DataMember]
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        #endregion
        #region Constructors and initializers

        public CopyTablesParameters()
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.items = null;
            this.timeout = 1200;    // *** TODO: get from settings
        }

        #endregion
    }
}
