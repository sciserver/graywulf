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

        protected Dictionary<string, FieldInfo> allProductions;

        protected Dictionary<string, FieldInfo> symbols;
        protected Dictionary<string, FieldInfo> terminals;
        protected Dictionary<string, FieldInfo> whitespaces;
        protected Dictionary<string, FieldInfo> comments;
        protected Dictionary<string, FieldInfo> rules;
        
        protected Dictionary<string, HashSet<string>> ruleDependencies;
        protected HashSet<string> keywords;
        protected GrammarInfo inheritedGrammar;
        protected HashSet<string> inheritedRules;

        public Type GrammarType
        {
            get { return grammarType; }
        }

        public GrammarAttribute Attributes
        {
            get { return attributes; }
        }

        public string ParserClassName
        {
            get { return attributes.ParserName ?? String.Format("{0}Parser", grammarType.Name); }
        }

        public string Namespace
        {
            get { return attributes.Namespace ?? grammarType.Namespace; }
        }

        public Dictionary<string, FieldInfo> AllProductions
        {
            get { return allProductions; }
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

        public GrammarInfo InheritedGrammar
        {
            get { return inheritedGrammar; }
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

            this.allProductions = null;

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
            allProductions = new Dictionary<string, FieldInfo>();

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
                    allProductions.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Terminal>))
                {
                    terminals.Add(f.Name, f);
                    allProductions.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Whitespace>))
                {
                    whitespaces.Add(f.Name, f);
                    allProductions.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Comment>))
                {
                    comments.Add(f.Name, f);
                    allProductions.Add(f.Name, f);
                }
                else if (f.FieldType == typeof(Expression<Grammar.Rule>))
                {
                    rules.Add(f.Name, f);
                    allProductions.Add(f.Name, f);
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

        public GrammarInfo FindProductionGrammar(string name)
        {
            if (allProductions.ContainsKey(name) || inheritedRules.Contains(name))
            {
                return this;
            }
            else if (inheritedGrammar != null)
            {
                return inheritedGrammar.FindProductionGrammar(name);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /*
        public Grammar FindSymbolsGrammar(string name)
        {
            if (symbols.ContainsKey(name))
            {
                return this;
            }
            else
            {
                foreach (Grammar g in EnumerateInheritedGrammars())
                {
                    if (g.Symbols.ContainsKey(name))
                    {
                        return g;
                    }
                }
            }

            return null;
        }

        public Grammar FindPatternsGrammar(string name)
        {
            if (patterns.ContainsKey(name))
            {
                return this;
            }
            else
            {
                foreach (Grammar g in EnumerateInheritedGrammars())
                {
                    if (g.Patterns.ContainsKey(name))
                    {
                        return g;
                    }
                }
            }

            return null;
        }
         * */

        public GrammarInfo FindProductionGrammar(string name, bool includeInherited)
        {
            if (allProductions.ContainsKey(name) || (includeInherited && inheritedRules.Contains(name)))
            {
                return this;
            }
            else
            {
                foreach (var g in EnumerateInheritedGrammars())
                {
                    if (g.allProductions.ContainsKey(name))
                    {
                        return g;
                    }
                }
            }

            return null;
        }

        public GrammarInfo FindProductionBaseGrammar(string name)
        {
            foreach (var g in EnumerateInheritedGrammars())
            {
                if (g.AllProductions.ContainsKey(name))
                {
                    return g;
                }
            }

            // Not found in base grammars
            return this;
        }

        /*
        public GrammarInfo FindRulesDefiningGrammar(string name)
        {
            foreach (var g in EnumerateInheritedGrammars())
            {
                if (g.Rules.ContainsKey(name))
                {
                    return g;
                }
            }

            return null;
        }*/
    }
}
