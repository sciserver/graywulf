using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class StarColumnIdentifierTest : ParsingTestBase
    {
        [TestMethod]
        public void SingleStarTest()
        {
            var sql = "*";
            var exp = new SqlParser().Execute<StarColumnIdentifier>(sql);
            Assert.AreEqual("*", exp.Value);
            Assert.IsTrue(exp.ColumnReference.IsStar);
        }

        [TestMethod]
        public void StarWithTablePrefixTest()
        {
            var sql = "schema1.table1.*";
            var exp = new SqlParser().Execute<StarColumnIdentifier>(sql);
            Assert.AreEqual("schema1.table1.*", exp.Value);
            Assert.AreEqual("schema1", exp.ColumnReference.TableReference.SchemaName);
            Assert.AreEqual("table1", exp.ColumnReference.TableReference.TableName);
            Assert.IsTrue(exp.ColumnReference.IsStar);
        }

        [TestMethod]
        public void CreateSingleStarTest()
        {
            var ci = StarColumnIdentifier.Create();
            Assert.AreEqual("*", ci.Value);
            Assert.IsTrue(ci.ColumnReference.IsStar);
            Assert.IsTrue(ci.ColumnReference.TableReference.IsUndefined);
        }

        [TestMethod]
        public void CreateTableStarTest()
        {
            var cg = CreateCodeGenerator();
            var tr = new NameResolution.TableReference()
            {
                TableName = "test"
            };
            var ci = StarColumnIdentifier.Create(tr);
            Assert.AreEqual("[test].*", cg.Execute(ci));
            Assert.IsTrue(ci.ColumnReference.IsStar);
            Assert.AreEqual("test", ci.ColumnReference.TableReference.TableName);
        }
    }
}
