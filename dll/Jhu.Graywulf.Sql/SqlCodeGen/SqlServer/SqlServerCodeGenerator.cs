using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlCodeGen.SqlServer
{
    public class SqlServerCodeGenerator : SqlCodeGeneratorBase
    {
        public static string GetCode(Node node, bool resolvedNames)
        {
            var sw = new StringWriter();
            var cg = new SqlServerCodeGenerator();
            cg.ResolveNames = resolvedNames;
            cg.Execute(sw, node);
            return sw.ToString();
        }

        public SqlServerCodeGenerator()
        {
        }

        #region Identifier formatting functions

        public static string QuoteIdentifier(string identifier)
        {
            return String.Format("[{0}]", identifier);
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

            if (!String.IsNullOrWhiteSpace(tableName))
            {
                // If no schema name is specified but there's a database name,
                // SQL Server uses the database..table syntax
                if (res != String.Empty)
                {
                    res += ".";
                }

                res += GetQuotedIdentifier(tableName);
            }

            return res;
        }

        protected override string GetResolvedFunctionName(string databaseName, string schemaName, string functionName)
        {
            string res = String.Empty;


            if (databaseName != null)
            {
                res += GetQuotedIdentifier(databaseName) + ".";
            }

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
                "SELECT {0} * FROM {1}",
                GenerateTopExpression(top),
                GetResolvedTableName(tableReference.DatabaseName, tableReference.SchemaName, tableReference.DatabaseObjectName));
        }

        public override string GenerateSelectStarQuery(TableOrView tableOrView, int top)
        {
            return String.Format(
                "SELECT {0} * FROM {1}",
                GenerateTopExpression(top),
                GetResolvedTableName(tableOrView.DatabaseName, tableOrView.SchemaName, tableOrView.ObjectName));
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

        #endregion
        #region Most restrictive query generation

        public override string GenerateMostRestrictiveTableQuery(QuerySpecification querySpecification, TableReference table, ColumnContext columnContext, int top)
        {
            // Run the normalizer to convert where clause to a normal form
            var cnr = new SearchConditionNormalizer();
            cnr.CollectConditions(querySpecification);
            var where = cnr.GenerateWhereClauseSpecificToTable(table);

            var columnlist = CreateColumnListGenerator(table, columnContext, ColumnListType.SelectWithOriginalNameNoAlias);
            columnlist.TableAlias = null;

            // Build table specific query
            var sql = new StringBuilder();

            sql.Append("SELECT ");

            if (top > 0)
            {
                sql.AppendFormat("TOP {0} ", top);
            }

            sql.AppendLine();
            sql.AppendLine(columnlist.Execute());
            sql.AppendFormat("FROM {0} ", GetResolvedTableName(table));

            if (!String.IsNullOrWhiteSpace(table.Alias))
            {
                sql.AppendFormat("AS {0} ", GetQuotedIdentifier(table.Alias));
            }

            sql.AppendLine();
            sql.AppendLine(Execute(where));

            return sql.ToString();
        }

        #endregion

        public override SqlColumnListGeneratorBase CreateColumnListGenerator(TableReference table, ColumnContext columnContext, ColumnListType listType)
        {
            var cl = new SqlServerColumnListGenerator(table.FilterColumnReferences(columnContext))
            {
                ListType = listType,
            };

            return cl;
        }

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
            sql.AppendLine(GetResolvedTableName((TableOrView)index.DatabaseObject));

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
                 LeadingSeparator = false
            };

            sql.Append("CREATE TABLE ");
            sql.Append(GetResolvedTableName(table));
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

            sql.AppendFormat("ALTER TABLE {0}", GetResolvedTableName(table));
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
                GetResolvedTableName(table),
                GetQuotedIdentifier(table.PrimaryKey.ObjectName));

            return sql;
        }

        public string GenerateDropIndexScript(Index index)
        {
            var sql =
@"DROP INDEX {0} ON {1}";

            sql = String.Format(sql,
                QuoteIdentifier(index.IndexName),
                GetResolvedTableName((TableOrView)index.DatabaseObject));

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

            sql.AppendFormat("ALTER TABLE {0}", GetResolvedTableName(table));
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
