using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class MethodName : IMethodReference
    {
        private MethodReference methodReference;

        public MethodReference MethodReference
        {
            get { return methodReference; }
            set { methodReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.methodReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (MethodName)other;
            this.methodReference = old.methodReference;
        }

        public override void Interpret()
        {
            base.Interpret();
            this.methodReference = MethodReference.Interpret(this);
        }

        public static MethodName Create(string methodName)
        {
            var id = Identifier.Create(methodName);
            var mn = new MethodName();
            mn.Stack.AddLast(id);
            return mn;
        }

        public static MethodName Create(MethodReference methodReference)
        {
            var mn = Create(methodReference.MethodName);
            mn.methodReference = methodReference;
            return mn;
        }
    }
}
