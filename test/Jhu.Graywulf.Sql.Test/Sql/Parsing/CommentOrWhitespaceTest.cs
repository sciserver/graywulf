using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class CommentOrWhitespaceTest
    {
        private Expression ExpressionTestHelper(string query)
        {
            var p = new SqlParser();
            return p.Execute<Expression>(query);
        }

        [TestMethod]
        public void SingleLineCommentTest()
        {
            var sql =
@"a + --this is a comment
b";
            var exp = ExpressionTestHelper(sql);
            var nodes = exp.Stack.ToArray();

            Assert.IsInstanceOfType(nodes[0], typeof(ColumnIdentifier));

            Assert.IsInstanceOfType(nodes[1], typeof(CommentOrWhitespace));
            Assert.AreEqual(" ", nodes[1].Value);

            Assert.IsInstanceOfType(nodes[2], typeof(ArithmeticOperator));
            Assert.AreEqual("+", nodes[2].Value);

            Assert.IsInstanceOfType(nodes[3], typeof(CommentOrWhitespace));
            Assert.AreEqual(" --this is a comment\r\n", nodes[3].Value);

            Assert.IsInstanceOfType(nodes[4], typeof(Expression));
        }

        [TestMethod]
        public void MultiLineCommentTest()
        {
            var sql =
@"a/*...*/+b";
            var exp = ExpressionTestHelper(sql);
            var nodes = exp.Stack.ToArray();

            Assert.IsInstanceOfType(nodes[0], typeof(ColumnIdentifier));

            Assert.IsInstanceOfType(nodes[1], typeof(CommentOrWhitespace));
            Assert.AreEqual("/*...*/", nodes[1].Value);

            Assert.IsInstanceOfType(nodes[2], typeof(ArithmeticOperator));
            Assert.AreEqual("+", nodes[2].Value);

            Assert.IsInstanceOfType(nodes[3], typeof(Expression));
            Assert.AreEqual("b", nodes[3].Value);
        }

        [TestMethod]
        public void MultiLineCommentTest2()
        {
            var sql =
@"a/*...
...*/+b";
            var exp = ExpressionTestHelper(sql);
            var nodes = exp.Stack.ToArray();

            Assert.IsInstanceOfType(nodes[0], typeof(ColumnIdentifier));

            Assert.IsInstanceOfType(nodes[1], typeof(CommentOrWhitespace));
            Assert.AreEqual("/*...\r\n...*/", nodes[1].Value);

            Assert.IsInstanceOfType(nodes[2], typeof(ArithmeticOperator));
            Assert.AreEqual("+", nodes[2].Value);

            Assert.IsInstanceOfType(nodes[3], typeof(Expression));
            Assert.AreEqual("b", nodes[3].Value);
        }

        [TestMethod]
        public void MultipleCommentsTest()
        {
            var sql = "/*c1*/ /*c2*/ -- c3";
            var exp = new SqlParser().Execute<CommentOrWhitespace>(sql);
        }
    }
}
