using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class VariableDeclaration : IVariableReference
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

        public DataTypeIdentifier DataType
        {
            get { return FindDescendant<DataTypeIdentifier>(); }
        }
        
        public bool IsCursor
        {
            get
            {
                var cursor = FindDescendant<Jhu.Graywulf.Parsing.Literal>(1);
                return cursor != null;
            }
        }

        public Expression Expression
        {
            get { return FindDescendant<Expression>(); }
        }

        public override void Interpret()
        {
            base.Interpret();
            Variable.VariableReference.InterpretVariableDeclaration(this);
        }
    }
}
