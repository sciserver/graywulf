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
using Jhu.Graywulf.Sql.QueryRendering;
using Jhu.Graywulf.Sql.QueryRendering.PostgreSql;

namespace Jhu.Graywulf.Sql.QueryGeneration.PostgreSql
{
    public class PostgreSqlQueryGenerator : QueryGeneratorBase
    {
        public PostgreSqlQueryGenerator()
        {
        }

        public override ColumnListGeneratorBase CreateColumnListGenerator()
        {
            return new PostgreSqlColumnListGenerator();
        }

        public override QueryRendererBase CreateQueryRenderer()
        {
            return new PostgreSqlQueryRenderer();
        }

        
        #region Complete query generators

        public override string GenerateSelectStarQuery(TableReference tableReference, int top)
        {
            return String.Format(
                "SELECT t.* FROM {1} AS t {0}",
                GenerateTopExpression(top),
                Renderer.GetResolvedTableName(tableReference.DatabaseName, tableReference.SchemaName, tableReference.DatabaseObjectName));
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
                Renderer.GetResolvedTableName(tableOrView.DatabaseName, tableOrView.SchemaName, tableOrView.ObjectName));
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
