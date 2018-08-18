using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Jhu.Graywulf.Parsing.Generator
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
        protected HashSet<string> overriddenRules;

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

        public HashSet<string> OverriddenRules
        {
            get { return overriddenRules; }
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
            CollectOverriddenRules();
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
            this.overriddenRules = null;
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
            if (includeCurrent && 
                (allRules.ContainsKey(name) || overriddenRules.Contains(name)))
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
        public GrammarInfo FindOverridingGrammar(string name)
        {
            if (allRules.ContainsKey(name) || overriddenRules.Contains(name))
            {
                return this;
            }
            else if (inheritedGrammar != null)
            {
                return inheritedGrammar.FindOverridingGrammar(name);
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
                // Collect all rules that are referenced from the production of the current rule
                var vis = new RuleVisitor();
                vis.Visit((Expression)rule.GetValue(null));

                //if (!vis.IsInheritedRule)
                //{
                    foreach (var dr in vis.ReferencedRules)
                    {
                        if (!ruleDependencies.ContainsKey(dr.Name))
                        {
                            ruleDependencies.Add(dr.Name, new HashSet<string>());
                        }

                        if (!ruleDependencies[dr.Name].Contains(rule.Name) && rule.Name != dr.Name)
                        {
                            ruleDependencies[dr.Name].Add(rule.Name);
                        }
                    }
                //}
            }
        }

        /// <summary>
        /// Identifies rules of the inherited grammars which contain anyting in the
        /// production that is overwritten by the current grammar. Match classes for
        /// these rules will automatically generated within the current namespace by
        /// inheriting from the parent grammar's match class.
        /// </summary>
        private void CollectOverriddenRules()
        {
            this.overriddenRules = new HashSet<string>();

            // Look for dependent non-terminals in inherited grammars
            // but not in the current grammar
            foreach (var g in EnumerateGrammars(false))
            {
                foreach (string rule in rules.Keys)
                {
                    var vis = new RuleVisitor();
                    vis.Visit((Expression)rules[rule].GetValue(null));

                    if (vis.IsOverrideRule)
                    {
                        CollectOverriddenRules(g, rule);
                    }
                }
            }
        }

        private void CollectOverriddenRules(GrammarInfo grammar, string rule)
        {
            // Find all rules in the grammar that have the passed rule in their production
            // These rules need to be inherited into the current namespace

            if (grammar.RuleDependencies.ContainsKey(rule))
            {
                foreach (var r in grammar.RuleDependencies[rule])
                {
                    // r has rule in its production list, so it needs to be inherited
                    // into the current namespace

                    if (!allRules.ContainsKey(r) && !overriddenRules.Contains(r))
                    {
                        overriddenRules.Add(r);

                        // Now do the whole thing recursively to get to the
                        // root node of the base grammar.
                        foreach (var g in grammar.EnumerateGrammars(true))
                        {
                            CollectOverriddenRules(g, r);
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
        public Expression GetRuleExpression(string name, out bool isAbstract, out bool isInherited, out GrammarInfo inheritedGrammar, out string inheritedRule)
        {
            // This is a rule that has to be inherited from the base grammar because
            // something in its production is overwritten
            if (overriddenRules.Contains(name))
            {
                isAbstract = false;
                return GetInheritedRuleExpression(name, out isInherited, out inheritedGrammar, out inheritedRule);
            }
            else
            {
                return GetLocalRuleExpression(name, out isAbstract, out isInherited, out inheritedGrammar, out inheritedRule);
            }
        }

        private Expression GetInheritedRuleExpression(string name, out bool isInherited, out GrammarInfo inheritedGrammar, out string inheritedRule)
        {
            isInherited = true;
            inheritedGrammar = FindDefiningGrammar(name, false);
            inheritedRule = name;

            /*var exp = inheritedGrammar.rules[name].GetValue(null) as LambdaExpression;
            var rule = exp.Body;

            return rule;*/

            var rule = inheritedGrammar.GetLocalRuleExpression(name, out var _, out var _, out var _, out var _);

            return rule;
        }

        private Expression GetLocalRuleExpression(string name, out bool isAbstract, out bool isInherited, out GrammarInfo inheritedGrammar, out string inheritedRule)
        {
            LambdaExpression exp = null;
            Expression rule = null;

            isAbstract = false;
            isInherited = false;
            inheritedGrammar = null;
            inheritedRule = null;

            exp = rules[name].GetValue(null) as LambdaExpression;
            rule = exp.Body;

            // This is not a rule but a terminal
            if (rule.NodeType != ExpressionType.Call)
            {
                return rule;
            }
            
            var method = ((MethodCallExpression)rule).Method;
            var args = ((MethodCallExpression)rule).Arguments.ToArray();

            if (rule != null && method != null && method.Name == nameof(Grammar.Abstract))
            {
                // No Match algorithm will be generated
                isAbstract = true;
                rule = null;

                if (args.Length == 0)
                {
                }
                else if (args.Length == 1)
                {
                    // Override the rule but keep abstract
                    inheritedRule = ((MemberExpression)args[0]).Member.Name;
                    inheritedGrammar = FindDefiningGrammar(inheritedRule, true);
                    isInherited = true;
                }
            }
            else if (rule != null && method != null && method.Name == nameof(Grammar.Override))
            {
                if (args.Length == 0)
                {
                    // Override the rule but inherit original production
                    inheritedGrammar = FindDefiningGrammar(name, false);
                    inheritedRule = name;
                    isInherited = true;
                    rule = null;
                }
                else if (args.Length == 1)
                {
                    // Override the rule with a new production
                    inheritedGrammar = FindDefiningGrammar(name, false);
                    inheritedRule = name;
                    isInherited = true;

                    // This is the new rule for which the match will be generated
                    rule = args[0];
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else if (rule != null && method != null && method.Name == nameof(Grammar.Inherit))
            {
                if (args.Length == 0)
                {
                    // Explicitly inherit rule into the current namespace with the same name
                    // and same production
                    inheritedGrammar = FindDefiningGrammar(name, false);
                    inheritedRule = name;
                    isInherited = true;
                    rule = null;
                }
                else if (args.Length == 1)
                {
                    // Inherit rule with a new name and the same production into the current namespace
                    inheritedRule = ((MemberExpression)args[0]).Member.Name;
                    inheritedGrammar = FindDefiningGrammar(inheritedRule, true);
                    isInherited = true;
                    rule = null;
                }
                else if (args.Length == 2)
                {
                    // Inherit rule with a new name and new production into the namespace
                    inheritedRule = ((MemberExpression)args[0]).Member.Name;
                    inheritedGrammar = FindDefiningGrammar(inheritedRule, true);
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
