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
using Jhu.Graywulf.Sql.CodeGeneration;
using Jhu.Graywulf.Sql.CodeGeneration.SqlServer;

namespace Jhu.Graywulf.Sql.CodeGeneration.SqlServer
{
    [TestClass]
    public class SqlServerColumnListGeneratorTest :SqlServerCodeGeneratorTestBase
    {

        private TableReference CreateTableReference()
        {
            var sql = @"SELECT Title, Year FROM Book b WHERE ID = 3";
            var ss = CreateSelect(sql);
            var tr = ss.EnumerateTableReferences().FirstOrDefault();

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
            Assert.AreEqual("[b].[_TEST_dbo_Book_b_Title], [b].[_TEST_dbo_Book_b_Year]", res);
        }

        [TestMethod]
        public void SelectWithOriginalNameTest()
        {
            var res = CreateList(ColumnListType.SelectWithOriginalName);
            Assert.AreEqual("[b].[Title] AS [_TEST_dbo_Book_b_Title], [b].[Year] AS [_TEST_dbo_Book_b_Year]", res);
        }

        [TestMethod]
        public void SelectWithEscapedNameTest()
        {
            var res = CreateList(ColumnListType.SelectWithEscapedName);
            Assert.AreEqual("[b].[_TEST_dbo_Book_b_Title], [b].[_TEST_dbo_Book_b_Year]", res);
        }

        [TestMethod]
        public void CreateTableWithOriginalNameTest()
        {
            var res = CreateList(ColumnListType.CreateTableWithOriginalName);
            Assert.AreEqual("[Title] nvarchar(50) NULL, [Year] int NULL", res);
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
    }
}
