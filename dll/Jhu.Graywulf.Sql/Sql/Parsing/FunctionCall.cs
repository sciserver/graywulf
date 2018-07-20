using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FunctionCall : IFunctionReference
    {
        public abstract FunctionReference FunctionReference { get; set; }

        public FunctionArguments FunctionArguments
        {
            get { return FindDescendant<FunctionArguments>(); }
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
