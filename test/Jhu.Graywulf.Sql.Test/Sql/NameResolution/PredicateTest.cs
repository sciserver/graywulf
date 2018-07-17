using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class PredicateTest : SqlNameResolverTestBase
    {
        private void GetSearchCondition(string query, out SelectStatement select, out BooleanExpression where)
        {
            select = ParseAndResolveNames<SelectStatement>(query);
            where = select.FindDescendantRecursive<WhereClause>().FindDescendant<BooleanExpression>();
        }

        [TestMethod]
        public void IsSpecificToTableTest()
        {
            // TODO: this won't work due to changes in predicate filtering logic
            // Figure out how to fix test
            // Move teste from parsing tests

            string sql;
            SelectStatement select;
            BooleanExpression where;
            TableReference table;
            Predicate predicate;

            // Simple predicate restricting a single table

            sql = "SELECT ID FROM Book WHERE ID = 6";
            GetSearchCondition(sql, out select, out where);

            table = select.SourceTableReferences.Values.First();
            predicate = where.FindDescendantRecursive<Predicate>();

            Assert.IsTrue(predicate.IsSpecificToTable(table));

            // Predicate that references multiple tables

            sql = "SELECT Book.ID FROM Book, Author WHERE Book.ID = Author.ID";
            GetSearchCondition(sql, out select, out where);

            table = select.SourceTableReferences.Values.First();
            predicate = where.FindDescendantRecursive<Predicate>();

            Assert.IsFalse(predicate.IsSpecificToTable(table));

            // Predicate that doesn't reference any tables

            sql = "SELECT ID FROM Book WHERE 4 = 6";
            GetSearchCondition(sql, out select, out where);

            table = select.SourceTableReferences.Values.First();
            predicate = where.FindDescendantRecursive<Predicate>();

            Assert.IsTrue(predicate.IsSpecificToTable(table));
        }
    }
}
