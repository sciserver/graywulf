using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryTraversal;
using Jhu.Graywulf.Sql.QueryGeneration;
using Jhu.Graywulf.Sql.QueryGeneration.SqlServer;

namespace Jhu.Graywulf.Sql.QueryGeneration.SqlServer
{
    [TestClass]
    public class SqlServerColumnListGeneratorTest : SqlServerTestBase
    {
        private TableReference CreateTableReference()
        {
            var sql = @"SELECT Title, Year FROM Book b WHERE ID = 3";
            var ss = ParseAndResolveNames<SelectStatement>(sql);
            var tr = ss.QueryExpression.FirstQuerySpecification.SourceTableReferences.Values.FirstOrDefault();

            return tr;
        }

        private string CreateList(ColumnListType listType)
        {
            var tr = CreateTableReference();
            var cg = new SqlServerColumnListGenerator(tr.FilterColumnReferences(ColumnContext.SelectList))
            {
                ListType = listType
            };

            return cg.Execute();
        }

        [TestMethod]
        public void SelectNoAliasTest()
        {
            var res = CreateList(ColumnListType.SelectWithEscapedNameNoAlias);
            Assert.AreEqual("[b].[__b_Title], [b].[__b_Year]", res);
        }

        [TestMethod]
        public void SelectWithOriginalNameTest()
        {
            var res = CreateList(ColumnListType.SelectWithOriginalName);
            Assert.AreEqual("[b].[Title] AS [__b_Title], [b].[Year] AS [__b_Year]", res);
        }

        [TestMethod]
        public void SelectWithEscapedNameTest()
        {
            var res = CreateList(ColumnListType.SelectWithEscapedName);
            Assert.AreEqual("[b].[__b_Title], [b].[__b_Year]", res);
        }

        [TestMethod]
        public void CreateTableWithOriginalNameTest()
        {
            var res = CreateList(ColumnListType.CreateTableWithOriginalName);
            Assert.AreEqual(
@"[Title] nvarchar(50) NULL,
[Year] int NULL", res);
        }

        [TestMethod]
        public void CreateViewTest()
        {
            var res = CreateList(ColumnListType.CreateView);
            Assert.AreEqual("[Title], [Year]", res);
        }

        [TestMethod]
        public void InsertTest()
        {
            var res = CreateList(ColumnListType.Insert);
            Assert.AreEqual("[Title], [Year]", res);
        }

        #region Column name escaping

        [TestMethod]
        public void EscapeColumnNameTest1()
        {
            var tr = new TableReference()
            {
                DatasetName = "TEST",
                SchemaName = "sch",
                DatabaseObjectName = "table",
                Alias = "a",
            };

            var gt = "a_col";

            var columns = new SqlServerColumnListGenerator();
            var res = (string)CallMethod(columns, "EscapeColumnName", tr, "col");

            Assert.AreEqual(gt, res);
        }

        [TestMethod]
        public void EscapeColumnNameTest2()
        {
            var tr = new TableReference()
            {
                DatasetName = "TEST",
                SchemaName = "sch",
                DatabaseObjectName = "table",
            };

            var gt = "TEST_sch_table_col";

            var columns = new SqlServerColumnListGenerator();
            var res = (string)CallMethod(columns, "EscapeColumnName", tr, "col");

            Assert.AreEqual(gt, res);
        }

        #endregion
    }
}
