using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Parsing
{
    public abstract class ParsingTreeVisitorBase
    {
        protected void ExecuteTopDown(Node node)
        {
            var res = VisitNode(node);

            if (!res)
            {
                foreach (var n in node.Stack)
                {
                    switch (n)
                    {
                        case Node nn:
                            ExecuteTopDown(nn);
                            break;
                        case Token t:
                            VisitToken(t);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        protected void ExecuteBottomUp(Node node)
        {
            foreach (var n in node.Stack)
            {
                switch (n)
                {
                    case Node nn:
                        ExecuteBottomUp(nn);
                        break;
                    case Token t:
                        VisitToken(t);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            var res = VisitNode(node);
        }

        /// <summary>
        /// When implemented in derived classes, processes the node and
        /// returns false if further descend into the parsing tree is
        /// necessary. This function should do the dispatching of
        /// tree nodes.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected abstract bool VisitNode(Node node);

        protected abstract void VisitToken(Token token);
    }
}
