using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Extensions.QueryRewriting
{
    public class GraywulfSqlQueryRewriterOptions
    {
        private bool substituteStars;
        private bool assignColumnAliases;
        private bool removeOrderyBy;
        private bool mapSystemVariables;
        private bool appendPartitioningConditions;
        private bool removePartitioningClause;

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

        public bool RemoveOrderBy
        {
            get { return removeOrderyBy; }
            set { removeOrderyBy = value; }
        }

        public bool MapSystemVariables
        {
            get { return mapSystemVariables; }
            set { mapSystemVariables = value; }
        }

        public bool AppendPartitioningCondition
        {
            get { return appendPartitioningConditions; }
            set { appendPartitioningConditions = value; }
        }

        public bool RemovePartitioningClause
        {
            get { return removePartitioningClause; }
            set { removePartitioningClause = value; }
        }

        public GraywulfSqlQueryRewriterOptions()
        {
            InitializeMembers();
        }

        public GraywulfSqlQueryRewriterOptions(GraywulfSqlQueryRewriterOptions old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.substituteStars = true;
            this.assignColumnAliases = true;
            this.removeOrderyBy = false;
            this.mapSystemVariables = true;
            this.appendPartitioningConditions = true;
            this.removePartitioningClause = true;
        }

        private void CopyMembers(GraywulfSqlQueryRewriterOptions old)
        {
            this.substituteStars = old.substituteStars;
            this.assignColumnAliases = old.assignColumnAliases;
            this.removeOrderyBy = old.removeOrderyBy;
            this.mapSystemVariables = old.mapSystemVariables;
            this.appendPartitioningConditions = old.appendPartitioningConditions;
            this.removePartitioningClause = old.removePartitioningClause;
        }
    }
}
