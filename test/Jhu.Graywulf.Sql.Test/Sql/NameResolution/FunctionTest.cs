using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class FunctionTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SystemFunctionCallTest()
        {
            var sql = "SELECT SIN(0.1)";
            var gt = "SELECT SIN(0.1)";

            var q = ParseAndResolveNames(sql);
            var res = GenerateCode(q.ParsingTree);

            Assert.AreEqual(gt, res);
            Assert.AreEqual(0, q.FunctionReferences.Count);
        }

        [TestMethod]
        public void AggregateFunctionCallTest()
        {
            Assert.Inconclusive();

            var sql = "SELECT MAX(ID) FROM Author";
            var q = ParseAndResolveNames(sql);

            Assert.AreEqual(1, q.FunctionReferences.Count);
        }

        [TestMethod]
        public void MemberAccessListTest()
        {
            var sql = "SELECT dbo.ScalarFunction()";
            var gt = "SELECT [Graywulf_Schema_Test].[dbo].[ScalarFunction]()";

            var q = ParseAndResolveNames(sql);
            var res = GenerateCode(q.ParsingTree);

            Assert.AreEqual(gt, res);
            Assert.AreEqual(1, q.FunctionReferences.Count);
        }

        [TestMethod]
        public void RankingFunctionCallTest()
        {
            var sql = "SELECT ROW_NUMBER() OVER (ORDER BY Name) FROM Author";
            var gt = "SELECT ROW_NUMBER() OVER (ORDER BY [Graywulf_Schema_Test].[dbo].[Author].[Name]) FROM [Graywulf_Schema_Test].[dbo].[Author]";

            var q = ParseAndResolveNames(sql);
            var res = GenerateCode(q.ParsingTree);

            Assert.AreEqual(gt, res);
            Assert.AreEqual(0, q.FunctionReferences.Count);
        }

        [TestMethod]
        public void WindowedFunctionCallTest2()
        {
            var sql = "SELECT dbo.ClrAggregateFunction(ID) OVER (Partition BY Name) FROM Author";
            var gt = "SELECT [Graywulf_Schema_Test].[dbo].[ClrAggregateFunction]([Graywulf_Schema_Test].[dbo].[Author].[ID]) OVER (Partition BY [Graywulf_Schema_Test].[dbo].[Author].[Name]) FROM [Graywulf_Schema_Test].[dbo].[Author]";

            var q = ParseAndResolveNames(sql);
            var res = GenerateCode(q.ParsingTree);

            Assert.AreEqual(gt, res);
            Assert.AreEqual(1, q.FunctionReferences.Count);
        }

        [TestMethod]
        public void TableValuedFunctionCallTest()
        {
            var sql = "SELECT a, b FROM TestTableValuedFunction() q";
            var gt = "SELECT [q].[a], [q].[b] FROM [Graywulf_Schema_Test].[dbo].[TestTableValuedFunction]() [q]";

            var q = ParseAndResolveNames(sql);
            var res = GenerateCode(q.ParsingTree);

            Assert.AreEqual(gt, res);
            Assert.AreEqual(1, q.FunctionReferences.Count);
        }

        [TestMethod]
        public void ClrTableValuedFunctionCallTest()
        {
            var sql = "SELECT ID FROM ClrTableValuedFunction() q";
            var gt = "SELECT [q].[ID] FROM [Graywulf_Schema_Test].[dbo].[ClrTableValuedFunction]() [q]";

            var q = ParseAndResolveNames(sql);
            var res = GenerateCode(q.ParsingTree);

            Assert.AreEqual(gt, res);
            Assert.AreEqual(1, q.FunctionReferences.Count);
        }
    }
}
