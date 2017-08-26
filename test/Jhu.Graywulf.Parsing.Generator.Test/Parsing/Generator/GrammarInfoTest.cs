using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CSharp;


namespace Jhu.Graywulf.Parsing.Generator
{
    [TestClass]
    public class GrammarInfoTest
    {

        [TestMethod]
        public void LoadGrammarTest()
        {
            var g = new GrammarInfo(typeof(TestGrammar));

            Assert.AreEqual(10, g.Rules.Count);
            Assert.AreEqual(10, g.RuleDependencies.Count);
            Assert.AreEqual("List", g.RuleDependencies["CommentOrWhitespace"].First());
            Assert.AreEqual("BaseRule2", g.RuleDependencies["BaseRule3"].First());
        }

        [TestMethod]
        public void LoadInheritedGrammarTest()
        {
            var g = new GrammarInfo(typeof(InheritedGrammar));

            Assert.AreEqual(1, g.EnumerateGrammars(false).Count());
            Assert.AreEqual(2, g.EnumerateGrammars(true).Count());
        }

        [TestMethod]
        public void CollectRuleDependenciesTest()
        {
            var g1 = new GrammarInfo(typeof(TestGrammar));

            Assert.AreEqual(10, g1.RuleDependencies.Count);

            Assert.AreEqual(2, g1.RuleDependencies["List"].Count);
            Assert.IsTrue(g1.RuleDependencies["List"].Contains("BaseRule1"));
            Assert.IsTrue(g1.RuleDependencies["List"].Contains("BaseRule5"));

            Assert.AreEqual(1, g1.RuleDependencies["BaseRule3"].Count);
            Assert.IsTrue(g1.RuleDependencies["BaseRule3"].Contains("BaseRule2"));

            Assert.AreEqual(1, g1.RuleDependencies["BaseRule4"].Count);
            Assert.IsTrue(g1.RuleDependencies["BaseRule4"].Contains("BaseRule3"));

            var g2 = new GrammarInfo(typeof(InheritedGrammar));

            Assert.AreEqual(4, g2.RuleDependencies.Count);

            Assert.AreEqual(1, g2.RuleDependencies["List"].Count);
            Assert.IsTrue(g2.RuleDependencies["List"].Contains("BaseRule1"));

            Assert.AreEqual(2, g2.RuleDependencies["BaseRule4"].Count);
            Assert.IsTrue(g2.RuleDependencies["BaseRule4"].Contains("BaseRule3"));
            Assert.IsTrue(g2.RuleDependencies["BaseRule4"].Contains("NewRule"));
        }

        [TestMethod]
        public void CollectOverriddenRulesTest()
        {
            var g1 = new GrammarInfo(typeof(TestGrammar));

            Assert.AreEqual(0, g1.OverriddenRules.Count);

            var g2 = new GrammarInfo(typeof(InheritedGrammar));

            Assert.AreEqual(2, g2.OverriddenRules.Count);
            Assert.IsTrue(g2.OverriddenRules.Contains("BaseRule2"));
            Assert.IsTrue(g2.OverriddenRules.Contains("BaseRule5"));
        }
    }
}
