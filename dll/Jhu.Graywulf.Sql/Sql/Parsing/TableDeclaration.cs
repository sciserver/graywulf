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

        public TableVariable Variable
        {
            get { return FindDescendant<TableVariable>(); }
        }
        
        public override void Interpret()
        {
            base.Interpret();
            Variable.VariableReference.InterpretTableDeclaration(this);
        }
    }
}
