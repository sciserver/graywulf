using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.LogicalExpressions;

namespace Jhu.Graywulf.Sql.CodeGeneration.MySql
{
    public class MySqlCodeGenerator : CodeGeneratorBase
    {
        public MySqlCodeGenerator()
        {
        }

        public override ColumnListGeneratorBase CreateColumnListGenerator()
        {
            return new MySqlColumnListGenerator();
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

        public override string GenerateSelectStarQuery(TableReference tableReference, string orderBy, long from, long max)
        {
            throw new NotImplementedException();
        }

        public override string GenerateSelectStarQuery(TableOrView tableOrView, int top)
        {
            return String.Format(
                "SELECT t.* FROM {1} AS t {0}",
                GenerateTopExpression(top),
                GetResolvedTableName(tableOrView.DatabaseName, tableOrView.SchemaName, tableOrView.ObjectName));
        }

        public override string GenerateSelectStarQuery(TableOrView tableOrView, string orderBy, long from, long max)
        {
            throw new NotImplementedException();
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

        protected override string GenerateOffsetExpression(long from, long max)
        {
            throw new NotImplementedException();
        }

        public override string GenerateMostRestrictiveTableQuery(string tableName, string tableAlias, string columnList, string where, int top)
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT ");
            sql.AppendLine(columnList);
            sql.AppendFormat(" FROM {0}", tableName);

            if (!String.IsNullOrWhiteSpace(tableAlias))
            {
                sql.AppendFormat("AS {0} ", tableAlias);
            }

            if (!String.IsNullOrWhiteSpace(where))
            {
                sql.AppendLine();
                sql.Append("WHERE ");
                sql.AppendLine(where);
            }

            if (top > 0)
            {
                sql.AppendFormat(" LIMIT {0} ", top);
            }

            return sql.ToString();
        }
        
        #endregion
    }
}
