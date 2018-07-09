using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class SqlQueryRewriterOptions
    {
        private bool appendPartitioning;
        private bool removePartitioning;
        private bool substituteStars;
        private bool assignColumnAliases;
        private bool removeOrderyBy;
        
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

        public bool SubstituteStars
        {
            get { return substituteStars; }
            set { substituteStars = value; }
        }

        public bool AssignColumnAliases
        {
            get { return assignColumnAliases; }
            set { assignColumnAliases = value; }
        }

        public SqlQueryRewriterOptions()
        {
            InitializeMembers();
        }

        public SqlQueryRewriterOptions(SqlQueryRewriterOptions old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.appendPartitioning = true;
            this.removePartitioning = true;
            this.substituteStars = true;
            this.assignColumnAliases = true;
    }

        private void CopyMembers(SqlQueryRewriterOptions old)
        {
            this.appendPartitioning = old.appendPartitioning;
            this.removePartitioning = old.removePartitioning;
            this.substituteStars = old.substituteStars;
            this.assignColumnAliases = old.assignColumnAliases;
        }
    }
}
