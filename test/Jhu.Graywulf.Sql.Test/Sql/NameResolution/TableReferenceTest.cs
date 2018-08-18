using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [TestClass]
    public class TableReferenceTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void TryMatchByReferenceTest()
        {
            var tr = new TableReference();
            Assert.IsTrue(tr.TryMatch(tr));
        }

        [TestMethod]
        public void TryMatchUndefinedTest()
        {
            var tr1 = new TableReference()
            {
                TableName = "test1"
            };
            var tr2 = new TableReference();
            var tr3 = new TableReference();

            // Undefined matches everything
            Assert.IsTrue(tr1.TryMatch(tr2));
            Assert.IsTrue(tr2.TryMatch(tr1));
            Assert.IsTrue(tr2.TryMatch(tr3));
        }

        [TestMethod]
        public void TryMatchByNameOnlyTest()
        {
            var tr1 = new TableReference()
            {
                TableName = "test1"
            };
            var tr2 = new TableReference()
            {
                TableName = "test1"
            };
            var tr3 = new TableReference()
            {
                TableName = "test2"
            };
            var tr4 = new TableReference();

            Assert.IsTrue(tr1.TryMatch(tr2));
            Assert.IsFalse(tr1.TryMatch(tr3));
            Assert.IsTrue(tr1.TryMatch(tr4));
            Assert.IsTrue(tr4.TryMatch(tr1));
        }

        [TestMethod]
        public void TryMatchByAliasOnlyTest()
        {
            var tr1 = new TableReference()
            {
                Alias = "test",
            };
            var tr2 = new TableReference()
            {
                Alias = "test",
            };
            var tr3 = new TableReference()
            {
                Alias = "test2",
            };
            var tr4 = new TableReference();

            Assert.IsTrue(tr1.TryMatch(tr2));
            Assert.IsFalse(tr1.TryMatch(tr3));
            Assert.IsTrue(tr1.TryMatch(tr4));
            Assert.IsTrue(tr4.TryMatch(tr1));
        }

        [TestMethod]
        public void TryMatchAliasOverrideTest()
        {
            // If an alias is set, it overrides everything
            var tr1 = new TableReference()
            {
                DatasetName = "dataset",
                DatabaseName = "database",
                SchemaName = "schame",
                TableName = "table",
            };
            var tr2 = new TableReference(tr1)
            {
                Alias = "alias2",
            };
            var tr3 = new TableReference(tr1)
            {
                Alias = "alias3",
            };
            var tr4 = new TableReference(tr3);

            Assert.IsFalse(tr1.TryMatch(tr2));
            Assert.IsTrue(tr2.TryMatch(tr1));
            Assert.IsFalse(tr2.TryMatch(tr3));
            Assert.IsTrue(tr3.TryMatch(tr4));
        }

        [TestMethod]
        public void TryMatchPartialNameTest()
        {
            var tr1 = new TableReference();
            var tr2 = new TableReference(tr1)
            {
                TableName = "table"
            };
            var tr3 = new TableReference(tr2)
            {
                SchemaName = "schema"
            };
            var tr4 = new TableReference(tr3)
            {
                DatabaseName = "database"
            };
            var tr5 = new TableReference(tr4)
            {
                DatasetName = "dataset"
            };
            var tr6 = new TableReference(tr5)
            {
                Alias = "alias"
            };

            // Partial name matches full name
            Assert.IsTrue(tr1.TryMatch(tr2));
            Assert.IsTrue(tr1.TryMatch(tr3));
            Assert.IsTrue(tr1.TryMatch(tr4));
            Assert.IsTrue(tr1.TryMatch(tr5));

            Assert.IsTrue(tr2.TryMatch(tr3));
            Assert.IsTrue(tr2.TryMatch(tr4));
            Assert.IsTrue(tr2.TryMatch(tr5));

            Assert.IsTrue(tr3.TryMatch(tr4));
            Assert.IsTrue(tr3.TryMatch(tr5));

            Assert.IsTrue(tr4.TryMatch(tr5));

            Assert.IsTrue(tr2.TryMatch(tr1));
            Assert.IsTrue(tr3.TryMatch(tr1));
            Assert.IsTrue(tr4.TryMatch(tr1));
            Assert.IsTrue(tr5.TryMatch(tr1));
        }
    }
}
