using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Jhu.Graywulf.Parsing.Generator
{
    class KeywordVisitor : ExpressionVisitor
    {
        private HashSet<string> keywords;

        public HashSet<string> Keywords
        {
            get { return keywords; }
        }

        public KeywordVisitor()
            : base()
        {
            keywords = new HashSet<string>();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(Grammar.Keyword))
            {
                var args = node.Arguments.Count;
                var keyword = (string)((ConstantExpression)node.Arguments[0]).Value;

                if (args == 1 || 
                    args == 2 && !((bool)((ConstantExpression)node.Arguments[1]).Value))
                {
                    // This is a global keyword

                    if (!keywords.Contains(keyword))
                    {
                        keywords.Add(keyword);
                    }
                }
            }

            return base.VisitMethodCall(node);
        }
    }
}
