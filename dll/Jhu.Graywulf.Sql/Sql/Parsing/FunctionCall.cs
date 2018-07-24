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


        // TODO: delete this, only used by unit tests
        public IEnumerable<Argument> EnumerateArguments()
        {
            var args = FindDescendant<FunctionArguments>();
            var list = args?.FindDescendant<ArgumentList>();

            if (list != null)
            {
                foreach (var arg in list.EnumerateArguments())
                {
                    yield return arg;
                }
            }
            else
            {
                yield break;
            }
        }
    }
}
