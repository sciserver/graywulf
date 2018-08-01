using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    [TestClass]
    public class SpecialFunctionTest
    {
        private string Execute(string code, ExpressionTraversalMethod expressionTraversal, ExpressionTraversalMethod logicalExpressionTraversal)
        {
            var exp = new SqlParser().Execute<Expression>(code);
            return new TestVisitorSink().Execute(exp, expressionTraversal, logicalExpressionTraversal);
        }

        [TestMethod]
        public void ConvertTest()
        {
            var exp = "2 + 3 * CONVERT(nvarchar(50), a + b)";

            var res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("2 + 3 * CONVERT ( nvarchar(50) , a + b ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Postfix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("2 3 nvarchar(50) , a b + CONVERT `2 * + ", res);

            res = Execute(exp, ExpressionTraversalMethod.Prefix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("+ 2 * 3 CONVERT `2 nvarchar(50) , + a b ", res);
        }

        [TestMethod]
        public void CastTest()
        {
            var exp = "2 + 3 * CAST(a + b AS nvarchar(50))";

            var res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("2 + 3 * CAST ( a + b AS nvarchar(50) ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Postfix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("2 3 a b + AS nvarchar(50) CAST `2 * + ", res);

            res = Execute(exp, ExpressionTraversalMethod.Prefix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("+ 2 * 3 CAST `2 + a b AS nvarchar(50) ", res);
        }

        [TestMethod]
        public void ParseTest()
        {
            var exp = "2 + 3 * PARSE(a + b AS nvarchar(50) USING '')";

            var res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("2 + 3 * PARSE ( a + b AS nvarchar(50) USING '' ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Postfix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("2 3 a b + AS nvarchar(50) USING '' PARSE `3 * + ", res);

            res = Execute(exp, ExpressionTraversalMethod.Prefix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("+ 2 * 3 PARSE `3 + a b AS nvarchar(50) USING '' ", res);
        }

        [TestMethod]
        public void DatePartTest()
        {
            var exp = "2 + 3 * DATEPART(year, datecol)";

            var res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("2 + 3 * DATEPART ( year , datecol ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Postfix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("2 3 year , datecol DATEPART `2 * + ", res);

            res = Execute(exp, ExpressionTraversalMethod.Prefix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("+ 2 * 3 DATEPART `2 year , datecol ", res);
        }

        [TestMethod]
        public void IifTest()
        {
            var exp = "IIF(a < b OR c > d AND e != f, a + b * c, d - e / f)";

            var res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Infix);
            Assert.AreEqual("IIF ( a < b OR c > d AND e != f , a + b * c , d - e / f ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Postfix);
            Assert.AreEqual("IIF ( a < b c > d e != f AND OR , a + b * c , d - e / f ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Infix, ExpressionTraversalMethod.Prefix);
            Assert.AreEqual("IIF ( OR a < b AND c > d e != f , a + b * c , d - e / f ) ", res);

            res = Execute(exp, ExpressionTraversalMethod.Postfix, ExpressionTraversalMethod.Postfix);
            Assert.AreEqual("a < b c > d e != f AND OR , a b c * + , d e f / - IIF `3 ", res);
        }
    }
}
