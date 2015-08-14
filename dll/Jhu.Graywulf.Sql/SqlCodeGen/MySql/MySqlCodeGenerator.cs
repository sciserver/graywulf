using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlCodeGen.MySql
{
    public class MySqlCodeGenerator : SqlCodeGeneratorBase
    {
        public MySqlCodeGenerator()
        {
        }

        #region Identifier formatting functions

        protected override string QuoteIdentifier(string identifier)
        {
            return String.Format("`{0}`", UnquoteIdentifier(identifier));
        }

        protected override string GetResolvedTableName(string databaseName, string schemaName, string tableName)
        {
            string res = String.Empty;

            if (!String.IsNullOrWhiteSpace(databaseName))
            {
                res += QuoteIdentifier(databaseName) + ".";
            }

            // MySQL doesn't have the equivalent of SQL Server schame name

            res += QuoteIdentifier(tableName);

            return res;
        }

        protected override string GetResolvedFunctionName(string databaseName, string schemaName, string functionName)
        {
            string res = String.Empty;


            if (databaseName != null)
            {
                res += QuoteIdentifier(databaseName) + ".";
            }

            // MySQL doesn't have the equivalent of SQL Server schame name

            res += QuoteIdentifier(functionName);

            return res;
        }

        #endregion
        #region Complete query generators

        public override string GenerateSelectStarQuery(TableOrView tableOrView, int top)
        {
            return String.Format(
                "SELECT t.* FROM {1} AS t {0}",
                GenerateTopExpression(top),
                GetResolvedTableName(tableOrView.DatabaseName, tableOrView.SchemaName, tableOrView.ObjectName));
        }

        protected override string GenerateTopExpression(int top)
        {
            string topstr = String.Empty;
            if (top < int.MaxValue)
            {
                topstr = String.Format("LIMIT {0}", top);
            }

            return topstr;
        }

        public override string GenerateMostRestrictiveTableQuery(QuerySpecification querySpecification, TableReference table, bool includePrimaryKey, int top)
        {
            // Normalize search conditions and extract where clause
            var cn = new SearchConditionNormalizer();
            cn.CollectConditions(querySpecification);
            var where = cn.GenerateWhereClauseSpecificToTable(table);

            // Build table specific query
            var sql = new StringWriter();

            sql.Write("SELECT ");

            // Now write the referenced columns
            int q = 0;
            foreach (var cr in table.ColumnReferences.Where(c => c.IsReferenced))
            {
                if (q != 0)
                {
                    sql.Write(", ");
                }

                if (cr.DataType.IsInteger)
                {
                    // Here a cast to a type that is accepted by SQL Server has to be made
                    sql.Write("CAST({0} AS SIGNED) AS {1}", GetResolvedColumnName(cr), QuoteIdentifier(cr.ColumnName));
                }
                else
                {
                    sql.Write(GetResolvedColumnName(cr));
                }
                q++;
            }

            // From cluse
            sql.Write(" FROM {0} ", GetResolvedTableName(table));
            if (table.Alias != null)
            {
                sql.Write("AS {0} ", QuoteIdentifier(table.Alias));
            }

            if (where != null)
            {
                Execute(sql, where);
            }

            if (top > 0)
            {
                sql.Write(" LIMIT {0} ", top);
            }

            return sql.ToString();
        }

        #endregion
    }
}
