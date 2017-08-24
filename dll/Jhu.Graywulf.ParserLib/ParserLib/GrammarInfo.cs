using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Jhu.Graywulf.ParserLib
{
    internal class GrammarInfo
    {
        #region Private member variables

        private Type grammarType;
        private GrammarAttribute attributes;

        protected GrammarInfo inheritedGrammar;

        protected Dictionary<string, FieldInfo> allRules;

        protected Dictionary<string, FieldInfo> symbols;
        protected Dictionary<string, FieldInfo> terminals;
        protected Dictionary<string, FieldInfo> whitespaces;
        protected Dictionary<string, FieldInfo> comments;
        protected Dictionary<string, FieldInfo> rules;

        protected HashSet<string> keywords;

        protected Dictionary<string, HashSet<string>> ruleDependencies;
        protected HashSet<string> overwrittenRules;

        #endregion
        #region Properties

        public Type GrammarType
        {
            get { return grammarType; }
        }

        public GrammarAttribute Attributes
        {
            get { return attributes; }
        }

        public GrammarInfo InheritedGrammar
        {
            get { return inheritedGrammar; }
        }

        public string ParserClassName
        {
            get { return attributes.ParserName ?? String.Format("{0}Parser", grammarType.Name); }
        }

        public string Namespace
        {
            get { return attributes.Namespace ?? grammarType.Namespace; }
        }

        public Dictionary<string, FieldInfo> AllRules
        {
            get { return allRules; }
        }

        public Dictionary<string, FieldInfo> Symbols
        {
            get { return symbols; }
        }

        public Dictionary<string, FieldInfo> Terminals
        {
            get { return terminals; }
        }

        public Dictionary<string, FieldInfo> Whitespaces
        {
            get { return whitespaces; }
        }

        public Dictionary<string, FieldInfo> Comments
        {
            get { return comments; }
        }

        public Dictionary<string, FieldInfo> Rules
        {
            get { return rules; }
        }

        public Dictionary<string, HashSet<string>> RuleDependencies
        {
            get { return ruleDependencies; }
        }

        public HashSet<string> Keywords
        {
            get { return keywords; }
        }

        public HashSet<string> OverwrittenRules
        {
            get { return overwrittenRules; }
        }

        #endregion
        #region Constructors and initializers

        public GrammarInfo(Type grammarType)
        {
            InitializeMembers();

            this.grammarType = grammarType;
            this.attributes = (GrammarAttribute)grammarType.GetCustomAttributes(typeof(GrammarAttribute), false)[0];

            LoadInheritedGrammar();

            CollectRules();
            CollectKeywords();
            CollectRuleDependencies();
            CollectOverwrittenRules();
        }

        private void InitializeMembers()
        {
            this.grammarType = null;
            this.attributes = null;

            this.allRules = null;

            this.symbols = null;
            this.terminals = null;
            this.whitespaces = null;
            this.comments = null;
            this.rules = null;

            this.ruleDependencies = null;
            this.keywords = null;
            this.inheritedGrammar = null;
            this.overwrittenRules = null;
        }

        #endregion
        #region Inherited grammar logic

        private void LoadInheritedGrammar()
        {
            if (grammarType.BaseType != typeof(Grammar))
            {
                inheritedGrammar = new GrammarInfo(grammarType.BaseType);
            }
        }

        public IEnumerable<GrammarInfo> EnumerateGrammars(bool includeCurrent)
        {
            if (includeCurrent)
            {
                yield return this;
            }

            if (inheritedGrammar != null)
            {
                foreach (var g in inheritedGrammar.EnumerateGrammars(true))
                {
                    yield return g;
                }
            }
        }

        /// <summary>
        /// Finds the first grammar in the inheritance chain that defines the rule.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GrammarInfo FindDefiningGrammar(string name, bool includeCurrent)
        {
            if (includeCurrent && allRules.ContainsKey(name))
            {
                return this;
            }
            else
            {
                foreach (var g in EnumerateGrammars(false))
                {
                    if (g.AllRules.ContainsKey(name))
                    {
                        return g;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Find the last grammar in the inheritance chain that overwrites the rule.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GrammarInfo FindOverwritingGrammar(string name)
        {
            if (allRules.ContainsKey(name) || overwrittenRules.Contains(name))
            {
                return this;
            }
            else if (inheritedGrammar != null)
            {
                return inheritedGrammar.FindOverwritingGrammar(name);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        #endregion
        #region Rule processing

        /// <summary>
        /// Analyzes the grammar class by looking at the public static
        /// fields of type Expression<T> and collect them.
        /// </summary>
        private void CollectRules()
        {
            allRules = new Dictionary<string, FieldInfo>();

            symbols = new Dictionary<string, FieldInfo>();
            terminals = new Dictionary<string, FieldInfo>();
            whitespaces = new Dictionary<string, FieldInfo>();
            comments = new Dictionary<string, FieldInfo>();
            rules = new Dictionary<string, FieldInfo>();

            foreach (var f in grammarType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (f.FieldType == typeof(Expression<Grammar.Symbol>))
                {
                    symbols.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Terminal>))
                {
                    terminals.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Whitespace>))
                {
                    whitespaces.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Comment>))
                {
                    comments.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Rule>))
                {
                    rules.Add(f.Name, f);
                }
                else
                {
                    throw new ParserGeneratorException(
                        String.Format(ExceptionMessages.UnrecognizedType,
                        f.Name, grammarType));
                }

                allRules.Add(f.Name, f);
            }
        }

        /// <summary>
        /// Identifies and collects keywords from the grammar.
        /// </summary>
        private void CollectKeywords()
        {
            keywords = new HashSet<string>();

            foreach (var f in rules.Values)
            {
                var vis = new KeywordVisitor();
                vis.Visit((Expression)f.GetValue(null));
                keywords.UnionWith(vis.Keywords);
            }

            if (inheritedGrammar != null)
            {
                keywords.UnionWith(inheritedGrammar.Keywords);
            }
        }

        /// <summary>
        /// Analyzes the grammar class and find dependecies between rules.
        /// A rule depends on another if it is referenced by the production
        /// of the other rule.
        /// </summary>
        private void CollectRuleDependencies()
        {
            ruleDependencies = new Dictionary<string, HashSet<string>>();

            foreach (var rule in rules.Values)
            {
                ruleDependencies.Add(rule.Name, new HashSet<string>());
            }

            foreach (var rule in rules.Values)
            {
                // Collect all rules that are referenced from the production of the current rule
                var vis = new RuleVisitor();
                vis.Visit((Expression)rule.GetValue(null));

                if (!vis.IsInheritedRule)
                {
                    foreach (var dr in vis.ReferencedRules)
                    {
                        if (rules.ContainsKey(dr.Name) && !ruleDependencies[dr.Name].Contains(rule.Name) && rule.Name != dr.Name)
                        {
                            ruleDependencies[dr.Name].Add(rule.Name);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Identifies rules of the inherited grammars which contain anyting in the
        /// production that is overwritten by the current grammar. Match classes for
        /// these rules will automatically generated within the current namespace by
        /// inheriting from the parent grammar's match class.
        /// </summary>
        private void CollectOverwrittenRules()
        {
            this.overwrittenRules = new HashSet<string>();

            // Look for dependent non-terminals in inherited grammars
            // but not in the current grammar
            foreach (var g in EnumerateGrammars(false))
            {
                foreach (string rule in rules.Keys)
                {
                    // If the rule appears anywhere in any production
                    // of the inherited grammar
                    if (g.RuleDependencies.ContainsKey(rule))
                    {
                        foreach (string inheritedRule in g.RuleDependencies[rule])
                        {
                            if (!allRules.ContainsKey(inheritedRule) && !overwrittenRules.Contains(inheritedRule))
                            {
                                overwrittenRules.Add(inheritedRule);
                                CollectInheritedRules(g, inheritedRule);
                            }
                        }
                    }
                }
            }
        }

        private void CollectInheritedRules(GrammarInfo grammar, string rule)
        {
            // Look for dependent non-terminals in inherited grammars
            foreach (var g in EnumerateGrammars(true))
            {
                if (g.RuleDependencies.ContainsKey(rule))
                {
                    foreach (string inheritedRule in g.RuleDependencies[rule])
                    {
                        if (!allRules.ContainsKey(inheritedRule) && !overwrittenRules.Contains(inheritedRule))
                        {
                            overwrittenRules.Add(inheritedRule);
                            CollectInheritedRules(g, inheritedRule);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Analyzes a rule directly defined in the grammar and determines whether it is
        /// inherited or not. If inherited from another grammar, the next grammar defining the rule
        /// in the inheritance chain is also returned.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isInherited"></param>
        /// <param name="isOverwritten"></param>
        /// <param name="inheritedGrammar"></param>
        /// <param name="inheritedRule"></param>
        /// <returns></returns>
        public Expression GetRuleExpression(string name, out bool isInherited, out bool isOverwritten, out GrammarInfo inheritedGrammar, out string inheritedRule)
        {
            // This is a rule that has to be inherited from the base grammar because
            // something in its production is overwritten
            if (overwrittenRules.Contains(name))
            {
                return GetInheritedRuleExpression(name, out isInherited, out isOverwritten, out inheritedGrammar, out inheritedRule);
            }
            else
            {
                return GetLocalRuleExpression(name, out isInherited, out isOverwritten, out inheritedGrammar, out inheritedRule);
            }
        }

        private Expression GetInheritedRuleExpression(string name, out bool isInherited, out bool isOverwritten, out GrammarInfo inheritedGrammar, out string inheritedRule)
        {
            isInherited = true;
            isOverwritten = true;
            inheritedGrammar = FindDefiningGrammar(name, false);
            inheritedRule = name;

            var exp = inheritedGrammar.rules[name].GetValue(null) as LambdaExpression;
            var rule = exp.Body;

            return rule;
        }

        private Expression GetLocalRuleExpression(string name, out bool isInherited, out bool isOverwritten, out GrammarInfo inheritedGrammar, out string inheritedRule)
        {
            LambdaExpression exp = null;
            Expression rule = null;

            isInherited = false;
            isOverwritten = false;
            inheritedGrammar = null;
            inheritedRule = null;

            exp = rules[name].GetValue(null) as LambdaExpression;
            rule = exp.Body;

            // This is not a rule but a terminal
            if (rule.NodeType != ExpressionType.Call)
            {
                isInherited = false;
                isOverwritten = true;
                return rule;
            }
            
            // There are four cases of rule inheritance:

            // 1. The rule is an Inherit() without parameters
            //    The match class is inherited into the new namespace with the same name but the
            //    rule is kept the original

            // 2. The rule is an Inherit(newproduction)
            //    In this case the match class is inherited into the new
            //    namespace and the match rule is overwritten

            // 3. The rule with the same name is defined in the inherited grammar
            //    In this case we throw an exception

            var method = ((MethodCallExpression)rule).Method;
            var args = ((MethodCallExpression)rule).Arguments.ToArray();

            if (rule != null && method != null && method.Name == "Inherit")
            {
                if (args.Length == 0)
                {
                    // Inherit rule with the same name
                    inheritedGrammar = FindDefiningGrammar(name, false);
                    inheritedRule = name;
                    isOverwritten = false;
                    isInherited = true;
                }
                else if (args.Length == 1)
                {
                    // Inherit rule with a new name
                    inheritedGrammar = FindDefiningGrammar(name, true);
                    inheritedRule = ((MemberExpression)args[0]).Member.Name;
                    isOverwritten = false;
                    isInherited = true;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else if (rule != null && method != null && method.Name == "Override")
            {
                if (args.Length == 1)
                {
                    // Inherit rule with the same name but overwrite production
                    inheritedGrammar = FindDefiningGrammar(name, false);
                    inheritedRule = name;
                    isOverwritten = true;
                    isInherited = true;

                    // This is the new rule for which the match will be generated
                    rule = args[0];
                }
                else if (args.Length == 2)
                {
                    // Inherit rule with a new name and override production
                    inheritedGrammar = FindDefiningGrammar(name, true);
                    inheritedRule = ((MemberExpression)args[0]).Member.Name;
                    isOverwritten = true;
                    isInherited = true;

                    // This is the new rule for which the match will be generated
                    rule = args[1];
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                // If it is a new rule, make sure it doesn't appear in any of the
                // base grammars
                inheritedGrammar = FindDefiningGrammar(name, true);

                if (inheritedGrammar != this)
                {
                    throw new ParserGeneratorException(
                        String.Format(ExceptionMessages.RuleExistsUseInherit,
                        name, grammarType.FullName));
                }

                isOverwritten = true;
                isInherited = false;
            }

            return rule;
        }

        #endregion

        public override string ToString()
        {
            return GrammarType.FullName;
        }
    }
}
