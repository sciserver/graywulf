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

        public override SqlColumnListGeneratorBase CreateColumnListGenerator(TableReference table, ColumnContext columnContext, ColumnListType listType)
        {
            var cl = new MySqlColumnListGenerator(table.FilterColumnReferences(columnContext))
            {
                ListType = listType
            };

            return cl;
        }

        #region Identifier formatting functions

        public static string QuoteIdentifier(string identifier)
        {
            return String.Format("`{0}`", identifier);
        }

        protected override string GetQuotedIdentifier(string identifier)
        {
            return QuoteIdentifier(UnquoteIdentifier(identifier));
        }

        protected override string GetResolvedTableName(string databaseName, string schemaName, string tableName)
        {
            string res = String.Empty;

            if (!String.IsNullOrWhiteSpace(databaseName))
            {
                res += GetQuotedIdentifier(databaseName) + ".";
            }

            // MySQL doesn't have the equivalent of SQL Server schame name

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

            // MySQL doesn't have the equivalent of SQL Server schame name

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
