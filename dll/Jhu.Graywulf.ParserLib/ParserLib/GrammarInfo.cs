using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Jhu.Graywulf.ParserLib
{
    public class GrammarInfo
    {
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

        protected HashSet<string> inheritedRules;
        protected Dictionary<string, HashSet<string>> ruleDependencies;
        
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
        
        public HashSet<string> InheritedRules
        {
            get { return inheritedRules; }
        }

        public GrammarInfo(Type grammarType)
        {
            InitializeMembers();

            this.grammarType = grammarType;
            this.attributes = (GrammarAttribute)grammarType.GetCustomAttributes(typeof(GrammarAttribute), false)[0];

            LoadInheritedGrammar();

            CollectProductions();
            CollectKeywords();
            CollectInheritedRules();
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
            this.inheritedRules = null;
        }

        private void CollectProductions()
        {
            allRules = new Dictionary<string, FieldInfo>();

            symbols = new Dictionary<string, FieldInfo>();
            terminals = new Dictionary<string, FieldInfo>();
            whitespaces = new Dictionary<string, FieldInfo>();
            comments = new Dictionary<string, FieldInfo>();
            rules = new Dictionary<string, FieldInfo>();
            
            ruleDependencies = new Dictionary<string, HashSet<string>>();

            // First path, collect symbols etc.
            foreach (var f in grammarType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (f.FieldType == typeof(Expression<Grammar.Symbol>))
                {
                    symbols.Add(f.Name, f);
                    allRules.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Terminal>))
                {
                    terminals.Add(f.Name, f);
                    allRules.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Whitespace>))
                {
                    whitespaces.Add(f.Name, f);
                    allRules.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Comment>))
                {
                    comments.Add(f.Name, f);
                    allRules.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Rule>))
                {
                    rules.Add(f.Name, f);
                    allRules.Add(f.Name, f);
                    ruleDependencies.Add(f.Name, new HashSet<string>());
                }
            }

            // Second pass, collect rule dependencies
            foreach (var f in rules.Values)
            {
                var vis = new RuleVisitor();
                vis.Visit((Expression)f.GetValue(null));

                foreach (var dr in vis.Rules)
                {
                    if (rules.ContainsKey(dr.Name) && !ruleDependencies[dr.Name].Contains(f.Name) && f.Name != dr.Name)
                    {
                        ruleDependencies[dr.Name].Add(f.Name);
                    }
                }
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

        private void CollectInheritedRules()
        {
            this.inheritedRules = new HashSet<string>();

            // Check if the rule can be inherited from the parent grammar or not.
            // In most cases it can be, but it there's anything in the production list
            // that is overloaded, then inheritance is not possible and a new rule class
            // needs to be generated.

            // The ruleDependencies collection ...

            // Look for dependent non-terminals in inherited grammars
            // but not in the current grammar
            foreach (var g in EnumerateInheritedGrammars())
            {
                foreach (string rule in this.Rules.Keys)
                {
                    if (g.RuleDependencies.ContainsKey(rule))
                    {
                        foreach (string inheritedrule in g.RuleDependencies[rule])
                        {
                            if (!inheritedRules.Contains(inheritedrule))
                            {
                                inheritedRules.Add(inheritedrule);
                                CollectInheritedRules(g, inheritedrule);
                            }
                        }
                    }
                }
            }
        }

        private void CollectInheritedRules(GrammarInfo grammar, string rule)
        {
            // Look for dependent non-terminals in inherited grammars
            foreach (var g in EnumerateGrammars())
            {
                if (g.RuleDependencies.ContainsKey(rule))
                {
                    foreach (string inheritedrule in g.RuleDependencies[rule])
                    {
                        if (!inheritedRules.Contains(inheritedrule))
                        {
                            inheritedRules.Add(inheritedrule);
                            CollectInheritedRules(g, inheritedrule);
                        }
                    }
                }
            }
        }

        private void LoadInheritedGrammar()
        {
            if (grammarType.BaseType != typeof(Grammar))
            {
                inheritedGrammar = new GrammarInfo(grammarType.BaseType);
            }
        }

        // ---

        public IEnumerable<GrammarInfo> EnumerateGrammars()
        {
            yield return this;

            if (inheritedGrammar != null)
            {
                foreach (var g in inheritedGrammar.EnumerateGrammars())
                {
                    yield return g;
                }
            }
        }

        public IEnumerable<GrammarInfo> EnumerateInheritedGrammars()
        {
            if (inheritedGrammar != null)
            {
                yield return inheritedGrammar;

                foreach (var g in inheritedGrammar.EnumerateInheritedGrammars())
                {
                    yield return g;
                }
            }
        }


        /// <summary>
        /// Find the first grammar in the ancestor chaing that defined the rule
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GrammarInfo FindRuleGrammar(string name)
        {
            if (allRules.ContainsKey(name) || inheritedRules.Contains(name))
            {
                return this;
            }
            else if (inheritedGrammar != null)
            {
                return inheritedGrammar.FindRuleGrammar(name);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        
        public GrammarInfo FindRuleGrammar(string name, bool includeInherited)
        {
            if (allRules.ContainsKey(name) || (includeInherited && inheritedRules.Contains(name)))
            {
                return this;
            }
            else
            {
                foreach (var g in EnumerateInheritedGrammars())
                {
                    if (g.allRules.ContainsKey(name))
                    {
                        return g;
                    }
                }
            }

            return null;
        }

        public GrammarInfo FindRuleBaseGrammar(string name)
        {
            foreach (var g in EnumerateInheritedGrammars())
            {
                if (g.AllRules.ContainsKey(name))
                {
                    return g;
                }
            }

            // Not found in base grammars
            return this;
        }
    }
}
