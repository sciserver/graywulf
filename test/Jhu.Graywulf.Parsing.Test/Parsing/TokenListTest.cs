using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Parsing.Test
{
    [TestClass]
    public class TokenListTest
    {
        [TestMethod]
        public void EnumerateForwardTest()
        {
            var tokens = new TokenList();

            for (int i = 0; i < 10; i++)
            {
                tokens.AddLast(new Keyword(i.ToString()));
            }

            int q = 0;
            foreach (var token in tokens.Forward)
            {
                Assert.AreEqual(q.ToString(), token.Value);
                q++;
            }
        }

        [TestMethod]
        public void EnumerateBackwardTest()
        {
            var tokens = new TokenList();

            for (int i = 0; i < 10; i++)
            {
                tokens.AddLast(new Keyword(i.ToString()));
            }

            int q = 0;
            foreach (var token in tokens.Backward)
            {
                Assert.AreEqual((9 - q).ToString(), token.Value);
                q++;
            }
        }

        [TestMethod]
        public void SingleItemTest()
        {
            var tokens = new TokenList();
            tokens.AddLast(new Keyword("0"));

            int q = 0;
            foreach (var token in tokens.Forward)
            {
                Assert.AreEqual("0", token.Value);
                q++;
            }
            Assert.AreEqual(1, q);

            q = 0;
            foreach (var token in tokens.Backward)
            {
                Assert.AreEqual("0", token.Value);
                q++;
            }
            Assert.AreEqual(1, q);
        }

        [TestMethod]
        public void NotItemTest()
        {
            var tokens = new TokenList();

            foreach (var token in tokens.Forward)
            {
                Assert.Fail();
            }

            foreach (var token in tokens.Backward)
            {
                Assert.Fail();
            }
        }
    }
}
