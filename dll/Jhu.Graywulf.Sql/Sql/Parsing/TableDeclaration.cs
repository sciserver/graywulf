using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TableDeclaration : IVariableReference
    {
        public VariableReference VariableReference
        {
            get { return Variable.VariableReference; }
            set { Variable.VariableReference = value; }
        }

        public UserVariable Variable
        {
            get { return FindDescendant<UserVariable>(); }
        }

        public TableDefinition TableDefinition
        {
            get { return FindDescendant<TableDefinition>(); }
        }

        public override void Interpret()
        {
            base.Interpret();
            VariableReference.Interpret(this);
        }
    }
}
