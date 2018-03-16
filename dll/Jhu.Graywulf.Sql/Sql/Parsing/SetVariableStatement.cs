using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SetVariableStatement : IStatement, IVariableReference
    {
        public bool IsResolvable
        {
            get { return true; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Command; }
        }

        public VariableReference VariableReference
        {
            get { return Variable.VariableReference; }
            set { Variable.VariableReference = value; }
        }

        public UserVariable Variable
        {
            get { return FindDescendant<UserVariable>(); }
        }

        public Expression Expression
        {
            get { return FindDescendant<Expression>(); }
        }

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield break;
        }
    }
}
