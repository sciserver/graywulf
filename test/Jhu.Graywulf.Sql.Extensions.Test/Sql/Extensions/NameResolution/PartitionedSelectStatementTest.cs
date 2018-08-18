using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Extensions.Parsing;

namespace Jhu.Graywulf.Sql.Extensions.NameResolution
{
    [TestClass]
    public class PartitionedSelectStatementTest : GraywulfSqlNameResolverTestBase
    {
        [TestMethod]
        public void ResolvePartitioningKeyTest1()
        {
            var sql = "SELECT * FROM Author PARTITION BY ID";
            var gt = "SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author] PARTITION BY [ID]";

            var exp = ParseAndResolveNames<SelectStatement>(sql);
            var res = GenerateCode(exp);
            
            Assert.AreEqual(gt, res);
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().Value);
            Assert.AreEqual("Author", exp.FindDescendantRecursive<PartitionedTableSourceSpecification>().TableReference.DatabaseObjectName);
            Assert.AreEqual("ID", exp.FindDescendantRecursive<PartitionedTableSourceSpecification>().PartitioningKeyExpression.Value);
        }

        [TestMethod]
        public void ResolvePartitioningKeyTest2()
        {
            var sql = "SELECT * FROM Author PARTITION BY Author.ID";
            var gt = "SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author] PARTITION BY [Author].[ID]";

            var exp = ParseAndResolveNames<SelectStatement>(sql);
            var res = GenerateCode(exp);

            Assert.AreEqual(gt, res);
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().Value);
            Assert.AreEqual("Author", exp.FindDescendantRecursive<PartitionedTableSourceSpecification>().TableReference.DatabaseObjectName);
            Assert.AreEqual("Author.ID", exp.FindDescendantRecursive<PartitionedTableSourceSpecification>().PartitioningKeyExpression.Value);
        }

        [TestMethod]
        public void ResolvePartitioningKeyWithAliasTest1()
        {
            var sql = "SELECT * FROM Author a PARTITION BY ID";
            var gt = "SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author] [a] PARTITION BY [ID]";

            var exp = ParseAndResolveNames<SelectStatement>(sql);
            var res = GenerateCode(exp);

            Assert.AreEqual(gt, res);
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().Value);
            Assert.AreEqual("Author", exp.FindDescendantRecursive<PartitionedTableSourceSpecification>().TableReference.DatabaseObjectName);
            Assert.AreEqual("ID", exp.FindDescendantRecursive<PartitionedTableSourceSpecification>().PartitioningKeyExpression.Value);
        }
        
        [TestMethod]
        public void ResolvePartitioningKeyWithAliasTest2()
        {
            var sql = "SELECT * FROM Author a PARTITION BY a.ID";
            var gt = "SELECT * FROM [Graywulf_Schema_Test].[dbo].[Author] [a] PARTITION BY [a].[ID]";

            var exp = ParseAndResolveNames<SelectStatement>(sql);
            var res = GenerateCode(exp);

            Assert.AreEqual(gt, res);
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().Value);
            Assert.AreEqual("Author", exp.FindDescendantRecursive<PartitionedTableSourceSpecification>().TableReference.DatabaseObjectName);
            Assert.AreEqual("a.ID", exp.FindDescendantRecursive<PartitionedTableSourceSpecification>().PartitioningKeyExpression.Value);
        }

        /*
        [TestMethod]
        public void SelectOrderByTest()
        {
            var sql = "SELECT * FROM table1 PARTITION BY ID ORDER BY a";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("*", exp.FindDescendantRecursive<SelectList>().Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableOrViewIdentifier>().Value);
            Assert.AreEqual("a", exp.FindDescendantRecursive<OrderByClause>().FindDescendantRecursive<Operand>().Value);
        }

        [TestMethod]
        [ExpectedException(typeof(Jhu.Graywulf.Parsing.ParserException))]
        public void SelectUnionQueryTest()
        {
            var sql =
@"SELECT a FROM table1 PARTITION BY ID
UNION
SELECT b FROM table2";

            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("UNION", exp.FindDescendantRecursive<QueryOperator>().Value);
        }

        [TestMethod]
        [ExpectedException(typeof(Jhu.Graywulf.Parsing.ParserException))]
        public void SelectUnionQueryOrderByTest()
        {
            var sql =
@"SELECT a FROM table1 PARTITION BY ID
UNION
SELECT b FROM table2
ORDER BY 1";

            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("UNION", exp.FindDescendantRecursive<QueryOperator>().Value);
        }

        [TestMethod]
        public void SelectDistinctTest()
        {
            var sql = "SELECT DISTINCT a FROM table1 PARTITION BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableOrViewIdentifier>().Value);
        }

        [TestMethod]
        public void SelectTopTest()
        {
            var sql = "SELECT TOP 10 a FROM table1 PARTITION BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("10", exp.FindDescendantRecursive<TopExpression>().FindDescendantRecursive<NumericConstant>().Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<TableOrViewIdentifier>().Value);
        }

        [TestMethod]
        public void SelectListTest()
        {
            var sql = "SELECT a, b, c, d FROM table1 PARTITION BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual(4, exp.FindDescendantRecursive<SelectList>().EnumerateDescendantsRecursive<ColumnExpression>().Count());
            Assert.AreEqual("a, b, c, d", exp.FindDescendantRecursive<SelectList>().Value);
        }

        [TestMethod]
        public void SelectIntoTest()
        {
            var sql = "SELECT a, b, c, d INTO table1 FROM table2 PARTITION BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("table1", exp.FindDescendantRecursive<IntoClause>().FindDescendantRecursive<TableOrViewIdentifier>().Value);
        }
        
        [TestMethod]
        public void SelectGroupByTest()
        {
            var sql = "SELECT AVG(a) FROM table1 PARTITION BY ID GROUP BY ID";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("ID", exp.FindDescendantRecursive<GroupByClause>().FindDescendant<GroupByList>().Value);
        }

        [TestMethod]
        public void SelectHavingTest()
        {
            var sql = "SELECT AVG(a) FROM table1 PARTITION BY ID HAVING AVG(ID) > 1";
            var exp = ExpressionTestHelper(sql);
            Assert.AreEqual(sql, exp.Value);
            Assert.AreEqual("AVG(ID) > 1", exp.FindDescendantRecursive<HavingClause>().FindDescendant<LogicalExpression>().Value);
        }
        */
    }
}
