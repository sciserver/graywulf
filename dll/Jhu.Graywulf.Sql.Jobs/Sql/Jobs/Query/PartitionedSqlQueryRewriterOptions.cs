using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class PartitionedSqlQueryRewriterOptions : Jhu.Graywulf.Sql.Extensions.QueryRewriting.GraywulfSqlQueryRewriterOptions
    {
        private bool appendPartitioning;
        private bool removePartitioning;
        
        public bool AppendPartitioning
        {
            get { return appendPartitioning; }
            set { appendPartitioning = value; }
        }

        public bool RemovePartitioning
        {
            get { return removePartitioning; }
            set { removePartitioning = value; }
        }

        public PartitionedSqlQueryRewriterOptions()
        {
            InitializeMembers();
        }

        public PartitionedSqlQueryRewriterOptions(PartitionedSqlQueryRewriterOptions old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.appendPartitioning = true;
            this.removePartitioning = true;
    }

        private void CopyMembers(PartitionedSqlQueryRewriterOptions old)
        {
            this.appendPartitioning = old.appendPartitioning;
            this.removePartitioning = old.removePartitioning;
        }
    }
}
