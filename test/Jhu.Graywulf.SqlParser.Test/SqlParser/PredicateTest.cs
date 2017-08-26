using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Schema;


namespace Jhu.Graywulf.SqlParser.Test
{
    [TestClass]
    public class PredicateTest
    {
        private void GetSearchCondition(string query, out SelectStatement select, out BooleanExpression where)
        {
            var p = new SqlParser();
            select = (SelectStatement)p.Execute(new SelectStatement(), query);
            where = select.FindDescendantRecursive<WhereClause>().FindDescendant<BooleanExpression>();
        }

        [TestMethod]
        public void BetweenTest()
        {
            SelectStatement select;
            BooleanExpression where;

            var sql = "SELECT ID FROM Book WHERE ID BETWEEN 6 AND 10";
            GetSearchCondition(sql, out select, out where);

            sql = "SELECT ID FROM Book WHERE ID BETWEEN 6 AND 10 AND Title = ''";
            GetSearchCondition(sql, out select, out where);
        }

        [TestMethod]
        public void IsSpecificToTableTest()
        {
            string sql;
            SelectStatement select;
            BooleanExpression where;
            TableReference table;
            Predicate predicate;
            
            // Simple predicate restricting a single table

            sql = "SELECT ID FROM Book WHERE ID = 6";           
            GetSearchCondition(sql, out select, out where);

            table = select.QueryExpression.EnumerateSourceTableReferences(false).First();
            predicate = where.FindDescendantRecursive<Predicate>();

            Assert.IsTrue(predicate.IsSpecificToTable(table));

            // Predicate that references multiple tables

            sql = "SELECT ID FROM Book, Author WHERE Book.ID = Author.ID";
            GetSearchCondition(sql, out select, out where);

            table = select.QueryExpression.EnumerateSourceTableReferences(false).First();
            predicate = where.FindDescendantRecursive<Predicate>();

            Assert.IsFalse(predicate.IsSpecificToTable(table));

            // Predicate that doesn't reference any tables

            sql = "SELECT ID FROM Book WHERE 4 = 6";
            GetSearchCondition(sql, out select, out where);

            table = select.QueryExpression.EnumerateSourceTableReferences(false).First();
            predicate = where.FindDescendantRecursive<Predicate>();

            Assert.IsTrue(predicate.IsSpecificToTable(table));
        }
    }
}
