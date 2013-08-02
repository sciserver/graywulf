using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Jhu.Graywulf.ParserLib
{
    class RuleVisitor : ExpressionVisitor
    {
        private HashSet<MemberInfo> rules;

        public HashSet<MemberInfo> Rules
        {
            get { return rules; }
        }

        public RuleVisitor()
            : base()
        {
            rules = new HashSet<MemberInfo>();
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            rules.Add(node.Member);

            return base.VisitMember(node);
        }
    }
}
