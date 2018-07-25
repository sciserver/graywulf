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
    public class MemberAccessListTest : SqlNameResolverTestBase
    {
        [TestMethod]
        public void SingleColumnTest()
        {
            var sql = "SELECT Name FROM Author";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
        }

        [TestMethod]
        public void SingleColumnWithPropertiesTest()
        {
            var sql = "SELECT Name.UdtProperty1.UdtProperty2 FROM Author";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name].[UdtProperty1].[UdtProperty2] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual(null, cs[0].ColumnName);
        }

        [TestMethod]
        public void SingleColumnWithMethodCallsTest()
        {
            var sql = "SELECT Name.UdtMethod1(Name, ID).UdtMethod2(12) FROM Author";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name].[UdtMethod1]([Graywulf_Schema_Test].[dbo].[Author].[Name], [Graywulf_Schema_Test].[dbo].[Author].[ID]).[UdtMethod2](12) FROM [Graywulf_Schema_Test].[dbo].[Author]", res);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual(null, cs[0].ColumnName);
        }

        [TestMethod]
        public void TableNameTest()
        {
            var sql = "SELECT Author.Name FROM Author";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
        }

        [TestMethod]
        public void SchemaAndTableNameTest()
        {
            var sql = "SELECT dbo.Author.Name FROM Author";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
        }

        [TestMethod]
        public void DatabaseSchemaAndTableNameTest()
        {
            var sql = "SELECT Graywulf_Schema_Test.dbo.Author.Name FROM Author";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[Author].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author]", res);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual(null, ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
        }

        // TODO: we could actually support missing schema name here...

        [TestMethod]
        public void TableAliasTest()
        {
            var sql = "SELECT a.Name FROM Author a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [a].[Name] FROM [Graywulf_Schema_Test].[dbo].[Author] [a]", res);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
        }

        [TestMethod]
        public void SchemaAndFunctionNameTest()
        {
            // This is handled by the FunctionName branch

            var sql = "SELECT dbo.ClrFunction(a.Name) FROM Author a";
            var qs = ParseAndResolveNames<QuerySpecification>(sql);

            var res = GenerateCode(qs);
            Assert.AreEqual("SELECT [Graywulf_Schema_Test].[dbo].[ClrFunction]([a].[Name]) FROM [Graywulf_Schema_Test].[dbo].[Author] [a]", res);

            var ts = qs.SourceTableReferences.Values.ToArray();

            Assert.AreEqual("Author", ts[0].DatabaseObjectName);
            Assert.AreEqual("a", ts[0].Alias);

            var cs = qs.ResultsTableReference.ColumnReferences.ToArray();

            Assert.AreEqual(1, cs.Length);
            Assert.AreEqual("Name", cs[0].ColumnName);
        }


    }
}
