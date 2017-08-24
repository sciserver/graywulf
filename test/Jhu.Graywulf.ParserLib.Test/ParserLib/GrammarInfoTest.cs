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


namespace Jhu.Graywulf.ParserLib.Test
{
    [TestClass]
    public class GrammarInfoTest
    {

        [TestMethod]
        public void LoadGrammarTest()
        {
            var g = new GrammarInfo(typeof(TestGrammar));

            Assert.AreEqual(9, g.Rules.Count);
            Assert.AreEqual(9, g.RuleDependencies.Count);
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
            Assert.AreEqual(1, g1.RuleDependencies["List"].Count);
            Assert.IsTrue(g1.RuleDependencies["List"].Contains("BaseRule1"));
            Assert.AreEqual(1, g1.RuleDependencies["BaseRule4"].Count);
            Assert.IsTrue(g1.RuleDependencies["BaseRule4"].Contains("BaseRule3"));

            var g2 = new GrammarInfo(typeof(InheritedGrammar));

            Assert.AreEqual(6, g2.RuleDependencies.Count);
            Assert.AreEqual(1, g2.RuleDependencies["List"].Count);
            Assert.IsTrue(g2.RuleDependencies["List"].Contains("BaseRule4"));
        }

        [TestMethod]
        public void CollectOverwrittenRulesTest()
        {
            var g1 = new GrammarInfo(typeof(TestGrammar));

            Assert.AreEqual(0, g1.OverwrittenRules.Count);

            var g2 = new GrammarInfo(typeof(InheritedGrammar));

            Assert.AreEqual(3, g2.OverwrittenRules.Count);
            Assert.IsTrue(g2.OverwrittenRules.Contains("BaseRule2"));
            Assert.IsTrue(g2.OverwrittenRules.Contains("BaseRule3"));
            Assert.IsTrue(g2.OverwrittenRules.Contains("BaseRule5"));
        }
    }
}
