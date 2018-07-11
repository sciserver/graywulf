using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Parsing
{
    public abstract class ParsingTreeVisitorBase
    {
        protected enum TraversalDirection
        {
            TopDown,
            BottomUp
        }

        private TraversalDirection direction;

        protected TraversalDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        protected void TraverseTopDown(Node node)
        {
            direction = TraversalDirection.TopDown;

            var res = VisitNode(node);

            if (!res)
            {
                foreach (var n in node.Stack)
                {
                    switch (n)
                    {
                        case Node nn:
                            TraverseTopDown(nn);
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

        protected void TraverseBottomUp(Node node)
        {
            direction = TraversalDirection.BottomUp;

            foreach (var n in node.Stack)
            {
                switch (n)
                {
                    case Node nn:
                        TraverseBottomUp(nn);
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
