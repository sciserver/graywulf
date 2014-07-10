using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Data
{
    public class RecordsetProperties : ICloneable
    {
        #region Private member variables

        private string name;
        private long recordCount;
        private DatabaseObjectMetadata metadata;
        private List<Column> columns;

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
            internal set { name = value; }
        }

        public long RecordCount
        {
            get { return recordCount; }
            internal set { recordCount = value; }
        }

        public DatabaseObjectMetadata Metadata
        {
            get { return metadata; }
            internal set { metadata = value; }
        }

        public List<Column> Columns
        {
            get { return columns; }
        }

        #endregion
        #region Constructors and initializers

        public RecordsetProperties()
        {
            InitializeMembers();
        }

        public RecordsetProperties(RecordsetProperties old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.recordCount = -1;
            this.metadata = null;
            this.columns = new List<Column>();
        }

        private void CopyMembers(RecordsetProperties old)
        {
            this.name = old.name;
            this.recordCount = old.recordCount;
            this.metadata = Util.DeepCloner.CloneObject(old.metadata);
            this.columns = new List<Column>(Util.DeepCloner.CloneCollection(old.columns));
        }

        public object Clone()
        {
            return new RecordsetProperties(this);
        }

        #endregion
    }
}
