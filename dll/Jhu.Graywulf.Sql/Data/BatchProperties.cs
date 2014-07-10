using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Data
{
    public class BatchProperties : ICloneable
    {
        #region Private member variables

        private string name;
        private DatasetMetadata metadata;

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
        }

        public DatasetMetadata Metadata
        {
            get { return metadata; }
        }

        #endregion
        #region Constructors and initializers

        public BatchProperties()
        {
            InitializeMembers();
        }

        public BatchProperties(BatchProperties old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.metadata = null;
        }

        private void CopyMembers(BatchProperties old)
        {
            this.name = old.name;
            this.metadata = Util.DeepCloner.CloneObject(old.metadata);
        }

        public object Clone()
        {
            return new BatchProperties(this);
        }

        #endregion
    }
}
