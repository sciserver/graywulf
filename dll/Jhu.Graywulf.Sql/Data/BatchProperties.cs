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

        #endregion
        #region Properties



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
