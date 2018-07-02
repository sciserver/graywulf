﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Parsing.Generator
{
    /// <summary>
    /// Represents a parser generator object that can be used to
    /// generate source code based on a compiled grammar.
    /// </summary>
    public class ParserGenerator
    {
        #region Private member variables

        /// <summary>
        /// Global counter used to generate unique variable names
        /// </summary>
        private int excount;

        #endregion

        /// <summary>
        /// Executes the source code generation
        /// </summary>
        /// <param name="output"></param>
        /// <param name="grammar"></param>
        public void Execute(TextWriter output, Type grammarType)
        {
            excount = 0;

            var grammar = new GrammarInfo(grammarType);
            var parser = GenerateParserClass(grammar);

            output.Write(parser.ToString());
        }

        private string GenerateParserClass(GrammarInfo grammar)
        {
            var template = new StringBuilder(Templates.Parser);

            template.Replace("__Namespace__", grammar.Namespace);
            template.Replace("__LibNamespace__", typeof(Token).Namespace);
            template.Replace("__Name__", grammar.ParserClassName);

            if (grammar.InheritedGrammar != null)
            {
                template.Replace("__InheritedType__",
                    String.Format(
                        "{0}.{1}",
                        grammar.InheritedGrammar.Namespace,
                        grammar.InheritedGrammar.ParserClassName));
            }
            else
            {
                template.Replace("__InheritedType__", typeof(Parser).FullName);
            }

            template.Replace("__Comparer__", grammar.Attributes.Comparer ?? "StringComparer.InvariantCultureIgnoreCase");
            template.Replace("__RootToken__", grammar.Attributes.RootToken);

            template.Replace("__Symbols__", GenerateSymbolClasses(grammar));
            template.Replace("__Terminals__", GenerateTerminalClasses(grammar));
            template.Replace("__Whitespaces__", GenerateWhitespaceClasses(grammar));
            template.Replace("__Comments__", GenerateCommentClasses(grammar));
            template.Replace("__Rules__", GenerateRuleClasses(grammar));

            template.Replace("__Keywords__", GenerateKeywordList(grammar));

            return template.ToString();
        }


        /// <summary>
        /// Generates a class for a symbol, terminal or whitespace
        /// </summary>
        /// <param name="template"></param>
        /// <param name="name"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private string GeneratePatternClass(StringBuilder template, string name, string pattern)
        {
            template.Replace("__LibNamespace__", typeof(Token).Namespace);
            template.Replace("__Name__", name);
            template.Replace("__Pattern__", pattern.Replace("\"", "\"\""));

            return template.ToString();
        }

        /// <summary>
        /// Generates all symbol classes
        /// </summary>
        /// <returns></returns>
        private string GenerateSymbolClasses(GrammarInfo grammar)
        {
            var sb = new StringBuilder();

            foreach (var name in grammar.Symbols.Keys)
            {
                sb.AppendLine(GenerateSymbolClass(grammar, name));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a single symbol class
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private string GenerateSymbolClass(GrammarInfo grammar, string name)
        {
            var template = new StringBuilder(Templates.Symbol);
            var exp = (LambdaExpression)grammar.Symbols[name].GetValue(null);
            var pattern = (string)((ConstantExpression)exp.Body).Value;

            return GeneratePatternClass(template, name, pattern);
        }

        /// <summary>
        /// Generates all terminal classes
        /// </summary>
        /// <returns></returns>
        private string GenerateTerminalClasses(GrammarInfo grammar)
        {
            var sb = new StringBuilder();

            foreach (var name in grammar.Terminals.Keys)
            {
                sb.AppendLine(GenerateTerminalClass(grammar, name));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a single terminal class
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private string GenerateTerminalClass(GrammarInfo grammar, string name)
        {
            var template = new StringBuilder(Templates.Terminal);

            var exp = (LambdaExpression)grammar.Terminals[name].GetValue(null);
            var pattern = (string)((ConstantExpression)exp.Body).Value;

            return GeneratePatternClass(template, name, pattern);
        }

        /// <summary>
        /// Generates all whitespace classes
        /// </summary>
        /// <returns></returns>
        private string GenerateWhitespaceClasses(GrammarInfo grammar)
        {
            var sb = new StringBuilder();

            foreach (var name in grammar.Whitespaces.Keys)
            {
                sb.AppendLine(GenerateWhitespaceClass(grammar, name));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a single whitespace class
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private string GenerateWhitespaceClass(GrammarInfo grammar, string name)
        {
            var template = new StringBuilder(Templates.Whitespace);

            var exp = (LambdaExpression)grammar.Whitespaces[name].GetValue(null);
            var pattern = (string)((ConstantExpression)exp.Body).Value;

            return GeneratePatternClass(template, name, pattern);
        }

        private string GenerateCommentClasses(GrammarInfo grammar)
        {
            var sb = new StringBuilder();

            foreach (var name in grammar.Comments.Keys)
            {
                sb.AppendLine(GenerateCommentClass(grammar, name));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private string GenerateCommentClass(GrammarInfo grammar, string name)
        {
            var template = new StringBuilder(Templates.Comment);

            var exp = (LambdaExpression)grammar.Comments[name].GetValue(null);
            var pattern = (string)((ConstantExpression)exp.Body).Value;

            return GeneratePatternClass(template, name, pattern);
        }

        /// <summary>
        /// Generates all classes for rules
        /// </summary>
        /// <returns></returns>
        private string GenerateRuleClasses(GrammarInfo grammar)
        {
            var sb = new StringBuilder();

            foreach (var name in grammar.Rules.Keys)
            {
                sb.AppendLine(GenerateRuleClass(grammar, name));
                sb.AppendLine();
            }

            // Classes that have to be inherited because they depend
            // on overwritten rules
            foreach (string name in grammar.OverriddenRules)
            {
                if (!grammar.AllRules.ContainsKey(name))
                {
                    sb.AppendLine(GenerateRuleClass(grammar, name));
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate a single rule class
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private string GenerateRuleClass(GrammarInfo grammar, string name)
        {
            int tabs = 3;
            var resstr = "res";

            // Figure out whether this rule is inherited or not 
            bool isInherited;
            GrammarInfo inheritedGrammar;
            string inheritedRule;
            string inheritedType;
            var rule = grammar.GetRuleExpression(name, out isInherited, out inheritedGrammar, out inheritedRule);

            // Generate matching code
            StringBuilder match = null;

            if (rule != null)
            {
                // In this case we generate a new match logic
                // This recreates the match logic even if the production of the rule didn't
                // changed during inheritance, because it might reference rules that did change
                // and in this case we have to use the class from the new namespace.
                // Generate the new match logic
                match = new StringBuilder(Templates.Match);
                match.Replace("__LibNamespace__", typeof(Token).Namespace);
                match.Replace("__Code__", GenerateRuleMatch(tabs, grammar, rule, resstr));
            }
            else
            {
                // Inherit the match logic from the base class
            }

            if (isInherited)
            {
                inheritedType = String.Format("{0}.{1}", inheritedGrammar.Namespace, inheritedRule);
            }
            else
            {
                inheritedType = String.Format("{0}.Node", typeof(Token).Namespace);
            }

            // Load template and substitute tokens
            var template = new StringBuilder(Templates.Rule);

            template.Replace("__InheritedType__", inheritedType);
            template.Replace("__LibNamespace__", typeof(Token).Namespace);
            template.Replace("__Namespace__", grammar.Namespace);
            template.Replace("__Name__", name);
            template.Replace("__Match__", match == null? "" : match.ToString());

            return template.ToString();
        }

        /// <summary>
        /// Generates code that matches a sequence, or alternatively anything else.
        /// </summary>
        /// <param name="tabs"></param>
        /// <param name="rule"></param>
        /// <param name="resstr"></param>
        /// <returns></returns>
        /// <remarks>
        /// If the matched rule is not a sequence it gets wrapped into a virtual sequence.
        /// </remarks>
        private string GenerateRuleMatch(int tabs, GrammarInfo grammar, Expression rule, string resstr)
        {
            var code = new CodeStringBuilder();

            if (rule.NodeType == ExpressionType.Call &&
                ((MethodCallExpression)rule).Method.Name == "Sequence")
            {
                // If it's a sequence extract arguments
                code.Append(GenerateSequenceMatch(tabs, grammar, (MethodCallExpression)rule, resstr));
            }
            else
            {
                // If it's not a sequence, wrap it in a list
                code.Append(GenerateListMatch(tabs, grammar, new Expression[] { rule }, resstr));
            }

            return code.ToString();
        }

        private string GenerateBaseMatch(int tabs)
        {
            CodeStringBuilder code = new CodeStringBuilder();
            code.AppendLineFormat(tabs, "return base.Match(parser);");
            return code.ToString();
        }

        /// <summary>
        /// Generates code to match a sequence.
        /// </summary>
        /// <param name="tabs"></param>
        /// <param name="exp"></param>
        /// <param name="resstr"></param>
        /// <returns></returns>
        private string GenerateSequenceMatch(int tabs, GrammarInfo grammar, MethodCallExpression exp, string resstr)
        {
            var args = (NewArrayExpression)exp.Arguments[0];
            return GenerateListMatch(tabs, grammar, args.Expressions, resstr);
        }

        /// <summary>
        /// Generates code to match Must and May constructs
        /// </summary>
        /// <param name="tabs"></param>
        /// <param name="exp"></param>
        /// <param name="must"></param>
        /// <param name="resstr"></param>
        /// <returns></returns>
        private string GenerateAlternativesMatch(int tabs, GrammarInfo grammar, MethodCallExpression exp, bool must, string resstr)
        {
            string exstr = String.Format("a{0}", ++excount);

            CodeStringBuilder code = new CodeStringBuilder();

            code.AppendLineFormat(tabs, "if ({0})", resstr);    //
            code.AppendLineFormat(tabs, "{{ // alternatives {0} {1}", exstr, must ? "must" : "may");
            code.AppendLineFormat(tabs + 1, "bool {0} = false;", exstr);

            var args = ((NewArrayExpression)exp.Arguments[0]).Expressions;

            foreach (var a in args)
            {
                code.AppendLineFormat(tabs + 1, "if (!{0})", exstr);
                code.Append(GenerateRuleMatch(tabs + 1, grammar, a, exstr));
            }

            if (must)
            {
                code.AppendLineFormat(tabs + 1, "{0} &= {1};", resstr, exstr);
            }
            else
            {
                code.AppendLineFormat(tabs + 1, "{0} |= {1};", resstr, exstr);
            }

            code.AppendLine();
            code.AppendLineFormat(tabs, "}} // end alternatives {0}", exstr);
            code.AppendLine();

            return code.ToString();
        }

        /// <summary>
        /// Generates code to match a list of rules
        /// </summary>
        /// <param name="tabs"></param>
        /// <param name="exp"></param>
        /// <param name="resstr"></param>
        /// <returns></returns>
        /// <remarks>
        /// This is used by the GenerateSequenceMatch and GenerateProductionMatch function
        /// to generate the inner part of the code matching sequences or any token wrapped
        /// into a virtual sequence.
        /// </remarks>
        private string GenerateListMatch(int tabs, GrammarInfo grammar, IEnumerable<Expression> exp, string resstr)
        {
            CodeStringBuilder code = new CodeStringBuilder();

            string exstr = String.Format("r{0}", ++excount);

            code.AppendLine(tabs, "{");
            GenerateCheckpoint(code, tabs + 1, exstr);
            code.AppendLine();
            code.AppendLineFormat(tabs + 1, "bool {0} = true;", exstr);

            foreach (var arg in exp)
            {
                switch (arg.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var m = (MemberExpression)arg;
                        if (m.Type == typeof(Expression<Grammar.Symbol>) ||
                            m.Type == typeof(Expression<Grammar.Terminal>) ||
                            m.Type == typeof(Expression<Grammar.Whitespace>) ||
                            m.Type == typeof(Expression<Grammar.Comment>) ||
                            m.Type == typeof(Expression<Grammar.Rule>))
                        {
                            code.AppendLineFormat(tabs + 1, "{0} = {0} && {1};", exstr, GenerateRuleMatch(grammar, m.Member.Name));
                        }
                        else
                        {
                            throw new InvalidOperationException();  //*** TODO
                        }
                        break;
                    case ExpressionType.Call:
                        var c = (MethodCallExpression)arg;
                        switch (c.Method.Name)
                        {
                            case "Sequence":
                                code.AppendLineFormat(tabs + 1, "if ({0})", exstr);
                                code.Append(GenerateSequenceMatch(tabs + 1, grammar, c, exstr));
                                break;
                            case "Must":
                                code.Append(GenerateAlternativesMatch(tabs + 1, grammar, c, true, exstr));
                                break;
                            case "May":
                                code.Append(GenerateMayMatch(tabs + 1, grammar, c, exstr));
                                break;
                            case "Literal":
                            case "Keyword":
                                code.AppendLineFormat(tabs + 1, "{0} = {0} && {1};", exstr, GenerateLiteralMatch(c));
                                break;
                            default:
                                throw new InvalidOperationException();  //*** TODO
                        }
                        break;
                    default:
                        throw new InvalidOperationException();  //*** TODO
                }
            }

            GenerateCommitOrRollback(code, tabs + 1, exstr);
            code.AppendLineFormat(tabs + 1, "{0} = {1};", resstr, exstr);
            code.AppendLine(tabs, "}");
            code.AppendLine();

            return code.ToString();
        }

        /// <summary>
        /// Generates code that matches a rule
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string GenerateRuleMatch(GrammarInfo grammar, string name)
        {
            var g = grammar.FindOverridingGrammar(name);

            //return String.Format("Match(parser, new {0}.{1}())", ns, name);
            return String.Format(
                "Match(parser, new {0}.{1}())",
                g.Namespace,
                name);
        }

        private string GenerateMayMatch(int tabs, GrammarInfo grammar, MethodCallExpression exp, string resstr)
        {
            string exstr = String.Format("a{0}", ++excount);

            CodeStringBuilder code = new CodeStringBuilder();

            var arg = exp.Arguments[0];

            code.AppendLineFormat(tabs, "if ({0})", resstr);
            code.AppendLineFormat(tabs, "{{ // may {0}", exstr);
            code.AppendLineFormat(tabs + 1, "bool {0} = false;", exstr);
            code.Append(GenerateRuleMatch(tabs + 1, grammar, arg, exstr));
            code.AppendLineFormat(tabs + 1, "{0} |= {1};", resstr, exstr);
            code.AppendLineFormat(tabs, "}} // end may {0}", exstr);
            code.AppendLine();

            return code.ToString();
        }

        /// <summary>
        /// Generates code that matches a keyword.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string GenerateLiteralMatch(MethodCallExpression exp)
        {
            var keyword = (string)((ConstantExpression)exp.Arguments[0]).Value;

            return String.Format("Match(parser, new {0}.{1}(@\"{2}\"))",
                typeof(Literal).Namespace,
                typeof(Literal).Name,
                keyword);
        }

        private void GenerateCheckpoint(CodeStringBuilder csb, int tabs, string exstr)
        {
            csb.AppendLineFormat(tabs, "Checkpoint(parser); // {0}", exstr);
        }

        private void GenerateCommitOrRollback(CodeStringBuilder csb, int tabs, string exstr)
        {
            csb.AppendLineFormat(tabs, "CommitOrRollback({0}, parser);", exstr);
        }

        private string GenerateKeywordList(GrammarInfo grammar)
        {
            var csb = new CodeStringBuilder();

            int q = 0;
            string line = "";
            foreach (var keyword in grammar.Keywords.OrderBy(i => i))
            {
                q++;

                if (q % 10 == 0)
                {
                    csb.AppendLine(3, line);
                    line = "";
                }

                line += String.Format("\"{0}\", ", keyword);
            }
            csb.AppendLine(3, line);

            return csb.ToString();
        }
    }
}
