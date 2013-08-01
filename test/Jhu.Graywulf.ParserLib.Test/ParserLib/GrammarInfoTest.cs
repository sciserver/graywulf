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

            Assert.AreEqual(2, g.Rules.Count);
            Assert.AreEqual(2, g.RuleDependencies.Count);
            Assert.AreEqual("List", g.RuleDependencies["CommentOrWhitespace"].First());
        }

    }
}
