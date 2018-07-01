using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SystemVariable : IVariableReference
    {
        private VariableReference variableReference;

        public VariableReference VariableReference
        {
            get { return variableReference; }
            set { variableReference = value; }
        }

        public string VariableName
        {
            get { return FindDescendant<Variable2>().Value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.variableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (SystemVariable)other;
            this.variableReference = old.variableReference;
        }

        public override void Interpret()
        {
            base.Interpret();
            this.variableReference = VariableReference.Interpret(this);
        }
    }
}
