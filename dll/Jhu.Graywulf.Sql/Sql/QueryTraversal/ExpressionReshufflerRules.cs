using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    abstract class ExpressionReshufflerRules
    {
        private ExpressionReshuffler reshuffler;

        public void Init(ExpressionReshuffler reshuffler)
        {
            this.reshuffler = reshuffler;
        }

        public abstract void Route(Token node);

        protected void Inline(Token node)
        {
            reshuffler.Inline(node);
            reshuffler.Output(node);
        }

        protected void Push(Node node)
        {
            reshuffler.Push(node);
        }

        protected void EmptyAndPush(Node node)
        {
            reshuffler.EmptyAndPush(node);
        }

        protected void Output(Token node)
        {
            reshuffler.Output(node);
        }
    }
}
