using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Jhu.Graywulf.ParserLib
{
    /// <summary>
    /// Overrides the ExpressionVisitor class to collect all
    /// rules that are referenced from a rule
    /// </summary>
    class RuleVisitor : ExpressionVisitor
    {
        private bool skipArgument;
        private bool isInheritRule;
        private bool isOverrideRule;
        private string inheritedRule;
        private HashSet<MemberInfo> referencedRules;
        
        public bool IsInheritedRule
        {
            get { return isInheritRule; }
        }

        public bool IsOverrideRule
        {
            get { return isOverrideRule; }
        }

        public string InheritedRule
        {
            get { return inheritedRule; }
        }

        public HashSet<MemberInfo> ReferencedRules
        {
            get { return referencedRules; }
        }

        public RuleVisitor()
            : base()
        {
            skipArgument = false;
            isInheritRule = false;
            isOverrideRule = false;
            inheritedRule = null;
            referencedRules = new HashSet<MemberInfo>();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            var args = node.Arguments.ToArray();

            if (method.Name == "Inherit")
            {
                isInheritRule = true;

                if (args.Length == 0)
                {
                    inheritedRule = null;
                    skipArgument = false;
                }
                else if (args.Length == 1)
                {
                    inheritedRule = ((MemberExpression)args[0]).Member.Name;
                    skipArgument = true;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else if (method.Name == "Override")
            {
                isOverrideRule = true;

                if (args.Length == 1)
                {
                    inheritedRule = null;
                    skipArgument = false;
                }
                else if (args.Length == 2)
                {
                    inheritedRule = ((MemberExpression)args[0]).Member.Name;
                    skipArgument = true;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                // This is just a standard rule like sequence, must, may etc.
                skipArgument = false;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (skipArgument)
            {
                // just skip this argument, it's the inherited rule
                skipArgument = false;
            }
            else
            {
                referencedRules.Add(node.Member);
            }

            return base.VisitMember(node);
        }
    }
}
