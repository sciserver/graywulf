using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    [TestClass]
    public class PredicateVisitorTest : LogicalExpressionsTestBase
    {
        [TestMethod]
        public void IsSpecificToTableTest()
        {
            string sql;
            QuerySpecification qs;
            LogicalExpression where;
            TableReference table;
            Parsing.Predicate predicate;
            PredicateVisitor visitor = new PredicateVisitor();

            // Simple predicate restricting a single table

            sql = "SELECT ID FROM Book WHERE ID = 6";
            GetSearchCondition(sql, out qs, out where);

            table = qs.SourceTableReferences.Values.First();
            predicate = where.FindDescendantRecursive<Parsing.Predicate>();

            Assert.IsTrue(visitor.IsSpecificToTable(predicate, table));

            // Predicate that references multiple tables

            sql = "SELECT Book.ID FROM Book, Author WHERE Book.ID = Author.ID";
            GetSearchCondition(sql, out qs, out where);

            table = qs.SourceTableReferences.Values.First();
            predicate = where.FindDescendantRecursive<Parsing.Predicate>();

            Assert.IsFalse(visitor.IsSpecificToTable(predicate, table));

            // Predicate that doesn't reference any tables

            sql = "SELECT ID FROM Book WHERE 4 = 6";
            GetSearchCondition(sql, out qs, out where);

            table = qs.SourceTableReferences.Values.First();
            predicate = where.FindDescendantRecursive<Parsing.Predicate>();

            Assert.IsTrue(visitor.IsSpecificToTable(predicate, table));
        }
    }
}
