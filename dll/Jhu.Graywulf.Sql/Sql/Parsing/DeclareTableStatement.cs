using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DeclareTableStatement : IStatement, IVariableReference
    {
        public bool IsResolvable
        {
            get { return true; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Declaration; }
        }

        public UserVariable TargetVariable
        {
            get { return FindDescendant<UserVariable>(); }
        }

        public VariableReference VariableReference
        {
            get { return TargetVariable.VariableReference; }
            set { TargetVariable.VariableReference = value; }
        }

        public TableDefinitionList TableDefinition
        {
            get { return FindDescendant<TableDefinitionList>(); }
        }

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield break;
        }
    }
}
