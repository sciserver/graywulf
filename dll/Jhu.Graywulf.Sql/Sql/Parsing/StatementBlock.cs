using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class StatementBlock : IStatement
    {
        #region Properties

        public bool IsResolvable
        {
            get { return false; }
        }
        
        #endregion
        
        public IEnumerable<Statement> EnumerateSubStatements()
        {
            return EnumerateDescendants<Statement>(true);
        }
    }
}
