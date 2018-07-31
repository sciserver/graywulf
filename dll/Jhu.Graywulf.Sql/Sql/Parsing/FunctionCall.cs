using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FunctionCall
    {
        private int argumentCount;

        public int ArgumentCount
        {
            get { return argumentCount; }
            set { argumentCount = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.argumentCount = 0;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (FunctionCall)other;
            this.argumentCount = old.argumentCount;
        }

        public Expression[] GetArguments()
        {
            var arglist = FindDescendant<ArgumentList>();
            var args = arglist == null ? new Expression[0] : arglist.EnumerateDescendants<Argument>().Select(a => a.Expression).ToArray();

            return args;
        }

        protected void AppendArguments(params Expression[] arguments)
        {
            var args = ArgumentList.Create(arguments);

            Stack.AddLast(BracketOpen.Create());

            if (args != null)
            {
                Stack.AddLast(args);
            }

            Stack.AddLast(BracketClose.Create());
        }
    }
}
