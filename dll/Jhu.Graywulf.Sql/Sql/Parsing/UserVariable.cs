using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UserVariable : IVariableReference
    {
        private VariableReference variableReference;

        public VariableReference VariableReference
        {
            get { return variableReference; }
            set { variableReference = value; }
        }

        public string VariableName
        {
            get { return FindDescendant<Variable>().Value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.variableReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (UserVariable)other;
            this.variableReference = old.variableReference;
        }

        public static UserVariable Create(string name)
        {
            var vv = Variable.Create(name);
            var res = new UserVariable(vv);
            
            res.variableReference = VariableReference.Interpret(res);
            
            return res;
        }

        public static UserVariable Create(VariableReference vr)
        {
            var res = new UserVariable()
            {
                variableReference = vr
            };
            var vv = Variable.Create(vr.VariableName);
            res.Stack.AddLast(vv);
            return res;
        }

        public override void Interpret()
        {
            base.Interpret();
            this.variableReference = VariableReference.Interpret(this);
        }
    }
}
