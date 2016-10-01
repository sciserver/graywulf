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

        public override SqlColumnListGeneratorBase CreateColumnListGenerator(TableReference table, ColumnContext columnContext, ColumnListType listType)
        {
            var cl = new PostgreSqlColumnListGenerator(table.FilterColumnReferences(columnContext))
            {
                ListType = listType
            };

            return cl;
        }

        #region Identifier formatting functions

        public static string QuoteIdentifier(string identifier)
        {
            return String.Format("\"{0}\"", identifier);
        }

        protected override string GetQuotedIdentifier(string identifier)
        {
            return QuoteIdentifier(identifier);
        }

        protected override string GetResolvedTableName(string databaseName, string schemaName, string tableName)
        {
            string res = String.Empty;

            if (!String.IsNullOrWhiteSpace(databaseName))
            {
                res += GetQuotedIdentifier(databaseName) + ".";
            }

            if (!String.IsNullOrWhiteSpace(schemaName))
            {
                res += GetQuotedIdentifier(schemaName);
            }

            // TODO: verify this for postgres!
            // If no schema name is specified but there's a database name,
            // SQL Server uses the database..table syntax
            if (res != String.Empty)
            {
                res += ".";
            }

            res += GetQuotedIdentifier(tableName);

            return res;
        }

        protected override string GetResolvedFunctionName(string databaseName, string schemaName, string functionName)
        {
            string res = String.Empty;


            if (databaseName != null)
            {
                res += GetQuotedIdentifier(databaseName) + ".";
            }

            // TODO: verify this for postgres
            // SQL Server function must always have the schema name specified
            res += GetQuotedIdentifier(schemaName) + ".";
            res += GetQuotedIdentifier(functionName);

            return res;
        }

        #endregion
        #region Complete query generators

        public override string GenerateSelectStarQuery(TableReference tableReference, int top)
        {
            return String.Format(
                "SELECT t.* FROM {1} AS t {0}",
                GenerateTopExpression(top),
                GetResolvedTableName(tableReference.DatabaseName, tableReference.SchemaName, tableReference.DatabaseObjectName));
        }

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
            if (top > 0 && top < int.MaxValue)
            {
                topstr = String.Format("LIMIT {0}", top);
            }

            return topstr;
        }

        public override string GenerateMostRestrictiveTableQuery(QuerySpecification querySpecification, TableReference table, ColumnContext columnContext, int top)
        {
            // Normalize search conditions and extract where clause
            var cn = new SearchConditionNormalizer();
            cn.CollectConditions(querySpecification);
            var where = cn.GenerateWhereClauseSpecificToTable(table);

            var columnlist = CreateColumnListGenerator(table, columnContext, ColumnListType.SelectWithOriginalNameNoAlias);

            // Build table specific query
            var sql = new StringBuilder();

            sql.AppendLine("SELECT ");
            sql.AppendLine(columnlist.Execute());
            sql.AppendFormat(" FROM {0}", GetQuotedIdentifier(table.DatabaseObjectName));
            
            if (!String.IsNullOrWhiteSpace(table.Alias))
            {
                sql.AppendFormat("AS {0} ", GetQuotedIdentifier(table.Alias));
            }
            
            sql.AppendLine();
            sql.AppendLine(Execute(where));

            if (top > 0)
            {
                sql.AppendFormat(" LIMIT {0} ", top);
            }

            return sql.ToString();
        }

        #endregion
    }
}
