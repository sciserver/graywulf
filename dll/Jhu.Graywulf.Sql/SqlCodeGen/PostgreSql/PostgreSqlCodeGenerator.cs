using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlCodeGen.PostgreSql
{
    public class PostgreSqlCodeGenerator : SqlCodeGeneratorBase
    {
        public PostgreSqlCodeGenerator()
        {
        }

        #region Identifier formatting functions

        protected override string QuoteIdentifier(string identifier)
        {
            return String.Format("\"{0}\"", identifier);
        }

        protected override string GetResolvedTableName(string databaseName, string schemaName, string tableName)
        {
            string res = String.Empty;

            if (!String.IsNullOrWhiteSpace(databaseName))
            {
                res += QuoteIdentifier(databaseName) + ".";
            }

            if (!String.IsNullOrWhiteSpace(schemaName))
            {
                res += QuoteIdentifier(schemaName);
            }

            // TODO: verify this for postgres!
            // If no schema name is specified but there's a database name,
            // SQL Server uses the database..table syntax
            if (res != String.Empty)
            {
                res += ".";
            }

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

            // TODO: verify this for postgres
            // SQL Server function must always have the schema name specified
            res += QuoteIdentifier(schemaName) + ".";
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

        public override string GenerateMostRestrictiveTableQuery(TableReference table, bool includePrimaryKey, int top)
        {
            // Normalize search conditions and extract where clause
            var cn = new SearchConditionNormalizer();
            cn.NormalizeQuerySpecification(((TableSource)table.Node).QuerySpecification);
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

                sql.Write("{0}", QuoteIdentifier(cr.ColumnName));
                q++;
            }

            // From cluse
            sql.Write(" FROM {0}", QuoteIdentifier(table.DatabaseObjectName));
            if (table.Alias != null)
            {
                sql.Write(" {0}", QuoteIdentifier(table.Alias));
            }
            sql.Write(" ");

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
