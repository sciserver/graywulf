using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing.NameResolver
{
    [TestClass]
    public class ScalarVariableTest : SqlNameResolverTestBase
    {
        private StatementBlock Parse(string sql)
        {
            var p = new SqlParser().Execute<StatementBlock>(sql);
            var nr = new SqlNameResolver();
            nr.Execute(p);
            return p;
        }

        [TestMethod]
        public void DeclareSingleVariableTest()
        {
            var script = Parse("DECLARE @var int");
            var d = script.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(1, script.VariableReferences.Count);
            Assert.IsTrue(script.VariableReferences.ContainsKey("@var"));
        }

        [TestMethod]
        public void DeclareMultipleVariableTest()
        {
            var script = Parse("DECLARE @var1 int, @var2 float");
            var d = script.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(2, script.VariableReferences.Count);
            Assert.IsTrue(script.VariableReferences.ContainsKey("@var1"));
            Assert.IsTrue(script.VariableReferences.ContainsKey("@var2"));

            script = Parse(
@"DECLARE @var1 int
DECLARE @var2 float");
            d = script.FindDescendantRecursive<VariableDeclaration>();
            Assert.AreEqual(2, script.VariableReferences.Count);
            Assert.IsTrue(script.VariableReferences.ContainsKey("@var1"));
            Assert.IsTrue(script.VariableReferences.ContainsKey("@var2"));
        }

        [TestMethod]
        [ExpectedException(typeof(NameResolverException))]
        public void DuplicateVariableTest()
        {
            var script = Parse("DECLARE @var1 int, @var1 float");
        }

        private void ReferencedVariableHelper(string name, string sql)
        {
            var script = Parse(sql);
            var s = script.FindDescendantRecursive<SelectStatement>();
            var v = s.FindDescendantRecursive<UserVariable>();
            Assert.AreEqual(script.VariableReferences[name], v.VariableReference);
        }

        [TestMethod]
        public void SelectListReferencedVariableTest()
        {
            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT @var1");

            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT @var1 = 6");

            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT @var1 AS vv");
        }

        [TestMethod]
        public void WhereClauseReferencedVariableTest()
        {
            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT 1 WHERE @var1 = 5");
        }

        [TestMethod]
        public void GroupByReferencedVariableTest()
        {
            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT 1 GROUP BY @var1");
        }

        [TestMethod]
        public void HavingReferencedVariableTest()
        {
            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT 1 GROUP BY 2 HAVING AVG(@var1) = 2");
        }

        [TestMethod]
        public void OrderByReferencedVariableTest()
        {
            ReferencedVariableHelper("@var1",
@"DECLARE @var1 int = 5
SELECT 1 ORDER BY @var1");
        }
        
    }
}
