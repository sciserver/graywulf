using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO.Tasks
{
    [Serializable]
    public abstract class TableSourceBase : ICloneable
    {
        protected TableSourceBase()
        {
            InitializeMembers();
        }

        protected TableSourceBase(TableSourceBase old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(TableSourceBase old)
        {
        }

        public abstract object Clone();

        internal abstract IDbConnection OpenConnection();

        internal abstract IDbCommand CreateCommand();
    }
}
