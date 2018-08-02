using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryRendering;
using Jhu.Graywulf.Sql.QueryRendering.SqlServer;
using Jhu.Graywulf.Sql.LogicalExpressions;

namespace Jhu.Graywulf.Sql.QueryGeneration.SqlServer
{
    public class SqlServerQueryGenerator : QueryGeneratorBase
    {
        public SqlServerQueryGenerator()
        {
        }

        public override ColumnListGeneratorBase CreateColumnListGenerator()
        {
            return new SqlServerColumnListGenerator();
        }

        public override QueryRendererBase CreateQueryRenderer()
        {
            return new SqlServerQueryRenderer()
            {
                Options = new QueryRendererOptions()
                {
                    TableNameRendering = NameRendering.FullyQualified,
                    TableAliasRendering = AliasRendering.Default,
                    ColumnNameRendering = NameRendering.FullyQualified,
                    ColumnAliasRendering = AliasRendering.Default,
                    DataTypeNameRendering = NameRendering.FullyQualified,
                    FunctionNameRendering = NameRendering.FullyQualified,
                    VariableRendering = VariableRendering.Substitute,
                }
            };
        }

        #region Identifier formatting functions

        public static string QuoteIdentifier(string identifier)
        {
            return String.Format("[{0}]", identifier);
        }

        #endregion
        #region Complete query generators

        public override string GenerateSelectStarQuery(TableReference tableReference, int top)
        {
            return String.Format(
                "SELECT {0} * FROM {1}",
                GenerateTopExpression(top),
                Renderer.GetResolvedTableName(tableReference.DatabaseName, tableReference.SchemaName, tableReference.DatabaseObjectName));
        }

        public override string GenerateSelectStarQuery(TableReference tableReference, string orderBy, long from, long max)
        {
            var offset = String.IsNullOrWhiteSpace(orderBy) ? null : GenerateOffsetExpression(from, max);

            return String.Format(
                "SELECT * FROM {0} {1} {2}",
                Renderer.GetResolvedTableName(tableReference.DatabaseName, tableReference.SchemaName, tableReference.DatabaseObjectName),
                orderBy,
                offset);
        }

        public override string GenerateSelectStarQuery(TableOrView tableOrView, int top)
        {
            return String.Format(
                "SELECT {0} * FROM {1}",
                GenerateTopExpression(top),
                Renderer.GetResolvedTableName(tableOrView.DatabaseName, tableOrView.SchemaName, tableOrView.ObjectName));
        }

        public override string GenerateSelectStarQuery(TableOrView tableOrView, string orderBy, long from, long max)
        {
            var offset = String.IsNullOrWhiteSpace(orderBy) ? null : GenerateOffsetExpression(from, max);

            return String.Format(
                "SELECT * FROM {0} {1} {2}",
                Renderer.GetResolvedTableName(tableOrView.DatabaseName, tableOrView.SchemaName, tableOrView.ObjectName),
                orderBy,
                offset);
        }

        protected override string GenerateTopExpression(int top)
        {
            var topstr = String.Empty;
            if (top > 0 && top < int.MaxValue)
            {
                topstr = String.Format("TOP {0}", top);
            }

            return topstr;
        }

        protected override string GenerateOffsetExpression(long from, long max)
        {
            string sql = "";

            if (from > 0)
            {
                sql += "OFFSET " + from + " ROWS";

                if (max > 0)
                {
                    sql += " FETCH NEXT " + max + " ROWS ONLY";
                }
            }

            return sql;
        }


        #endregion
        #region Most restrictive query generation

        public override string GenerateMostRestrictiveTableQuery(string tableName, string tableAlias, string columnList, string where, int top)
        {
            var sql = new StringBuilder();

            sql.Append("SELECT ");

            if (top > 0)
            {
                sql.AppendFormat("TOP {0} ", top);
            }

            sql.AppendLine();
            sql.AppendLine(columnList);
            sql.AppendFormat("FROM {0} ", tableName);

            if (!String.IsNullOrWhiteSpace(tableAlias))
            {
                sql.AppendFormat("AS {0} ", Renderer.GetQuotedIdentifier(tableAlias));
            }

            if (!String.IsNullOrWhiteSpace(where))
            {
                sql.AppendLine();
                sql.Append("WHERE ");
                sql.AppendLine(where);
            }

            return sql.ToString();
        }

        #endregion

        private void GenerateIndexColumns(StringBuilder sql, Index index)
        {
            var columnList = new SqlServerColumnListGenerator(index)
            {
                ListType = ColumnListType.CreateIndex
            };

            sql.AppendLine(" (");
            columnList.Execute(sql);
            sql.AppendLine();
            sql.AppendLine(" )");
        }

        private void GenerateIndexOptions(StringBuilder sql, Index index, bool sortInTempDb)
        {
            sql.AppendLine("WITH (");
            sql.Append("DATA_COMPRESSION = " + (index.IsCompressed ? "PAGE" : "NONE"));
            if (!index.IsPrimaryKey)
            {
                sql.AppendLine(",");
                sql.Append("SORT_IN_TEMPDB = " + (sortInTempDb ? "ON" : "OFF"));
            }
            sql.AppendLine(")");
        }

        private void GenerateCreateIndexScript(StringBuilder sql, Index index, bool sortInTempDb)
        {
            sql.Append("CREATE");
            sql.Append(index.IsUnique ? " UNIQUE" : "");
            sql.Append(index.IsClustered ? " CLUSTERED" : " NONCLUSTERED");
            sql.Append(" INDEX ");
            sql.AppendLine(QuoteIdentifier(index.IndexName));
            sql.Append("ON ");
            sql.AppendLine(Renderer.GetResolvedTableName((TableOrView)index.DatabaseObject));

            GenerateIndexColumns(sql, index);
            GenerateIndexOptions(sql, index, sortInTempDb);
        }

        private void GeneratePrimaryKeyConstraint(StringBuilder sql, TableOrView table, bool sortInTempDb)
        {
            var index = table.PrimaryKey;

            sql.Append("CONSTRAINT ");
            sql.Append(QuoteIdentifier(index.IndexName));
            sql.Append(" PRIMARY KEY ");
            sql.Append(index.IsClustered ? "CLUSTERED" : "NONCLUSTERED");

            GenerateIndexColumns(sql, index);
        }

        public string GenerateCreateTableScript(Table table, bool generatePrimaryKey, bool generateIndexes)
        {
            return GenerateCreateTableScript(table, generatePrimaryKey, generateIndexes, true);
        }

        public string GenerateCreateTableScript(Table table, bool generatePrimaryKey, bool generateIndexes, bool sortInTempDb)
        {
            if (table.Columns.Count == 0)
            {
                throw new InvalidOperationException("The table doesn't have any columns.");     // TODO ***
            }

            var sql = new StringBuilder();
            var columns = new SqlServerColumnListGenerator(table)
            {
                ListType = ColumnListType.CreateTableWithOriginalName,
                SeparatorRendering = ColumnListSeparatorRendering.Default
            };

            sql.Append("CREATE TABLE ");
            sql.Append(Renderer.GetResolvedTableName(table));
            sql.AppendLine(" (");
            columns.Execute(sql);

            if (generatePrimaryKey && table.PrimaryKey != null)
            {
                sql.AppendLine(",");
                GeneratePrimaryKeyConstraint(sql, table, sortInTempDb);
            }

            sql.AppendLine(" )");

            if (generatePrimaryKey && table.PrimaryKey != null)
            {
                GenerateIndexOptions(sql, table.PrimaryKey, sortInTempDb);
            }

            if (generateIndexes)
            {
                foreach (var index in table.Indexes.Values)
                {
                    if (!index.IsPrimaryKey)
                    {
                        GenerateCreateIndexScript(sql, index, sortInTempDb);
                    }
                }
            }

            return sql.ToString();
        }

        public string GenerateCreatePrimaryKeyScript(TableOrView table)
        {
            return GenerateCreatePrimaryKeyScript(table, true);
        }

        public string GenerateCreatePrimaryKeyScript(TableOrView table, bool sortInTempDb)
        {
            if (table.Columns.Count == 0)
            {
                throw new InvalidOperationException("The table doesn't have any columns.");     // TODO ***
            }

            if (table.PrimaryKey == null)
            {
                throw new InvalidOperationException("The table doesn't have a primary key defined.");     // TODO ***
            }

            if (table.PrimaryKey.Columns.Count == 0)
            {
                throw new InvalidOperationException("There are no columns defined for primary key.");     // TODO ***
            }

            var sql = new StringBuilder();

            sql.AppendFormat("ALTER TABLE {0}", Renderer.GetResolvedTableName(table));
            sql.AppendLine();
            sql.Append("ADD ");
            GeneratePrimaryKeyConstraint(sql, table, sortInTempDb);

            return sql.ToString();
        }

        public string GenerateCreateIndexScript(Index index)
        {
            return GenerateCreateIndexScript(index, true);
        }

        public string GenerateCreateIndexScript(Index index, bool sortInTempDb)
        {
            var table = (TableOrView)index.DatabaseObject;

            if (table == null)
            {
                throw new InvalidOperationException("The table is null.");     // TODO ***
            }

            if (table.Columns.Count == 0)
            {
                throw new InvalidOperationException("The table doesn't have any columns.");     // TODO ***
            }

            var sql = new StringBuilder();

            GenerateCreateIndexScript(sql, index, sortInTempDb);

            return sql.ToString();
        }

        public string GenerateDropPrimaryKeyScript(TableOrView table)
        {
            var sql =
@"ALTER TABLE {0}
DROP CONSTRAINT {1}";

            sql = String.Format(sql,
                Renderer.GetResolvedTableName(table),
                Renderer.GetQuotedIdentifier(table.PrimaryKey.ObjectName));

            return sql;
        }

        public string GenerateDropIndexScript(Index index)
        {
            var sql =
@"DROP INDEX {0} ON {1}";

            sql = String.Format(sql,
                QuoteIdentifier(index.IndexName),
                Renderer.GetResolvedTableName((TableOrView)index.DatabaseObject));

            return sql;
        }

        private string GenerateAddColumnEntry(Column column)
        {
            return String.Format("ADD {0} {1} {2} {3}",
                QuoteIdentifier(column.Name),
                column.DataType.TypeNameWithLength,
                column.DataType.IsNullable ? "NULL" : "NOT NULL",
                column.IsIdentity ? "IDENTITY (1, 1)" : "");
        }

        public string GenerateAddColumnScript(Table table, IList<Column> columns)
        {
            var sql = new StringBuilder();

            sql.AppendFormat("ALTER TABLE {0}", Renderer.GetResolvedTableName(table));
            sql.AppendLine();

            foreach (var column in columns)
            {
                sql.AppendLine(GenerateAddColumnEntry(column));
            }

            return sql.ToString();
        }

        public string GenerateDropColumnScript(Table table, IList<Column> columns)
        {
            throw new NotImplementedException();
        }
    }
}
