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

        protected override string QuoteIdentifier(string identifier)
        {
            return String.Format("[{0}]", identifier);
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
                "SELECT {0} * FROM {1}",
                GenerateTopExpression(top),
                GetResolvedTableName(tableOrView.DatabaseName, tableOrView.SchemaName, tableOrView.ObjectName));
        }

        protected override string GenerateTopExpression(int top)
        {
            var topstr = String.Empty;
            if (top < int.MaxValue)
            {
                topstr = String.Format("TOP {0}", top);
            }

            return topstr;
        }

        public override string GenerateMostRestrictiveTableQuery(QuerySpecification querySpecification, TableReference table, bool includePrimaryKey, int top)
        {
            // Run the normalizer to convert where clause to a normal form
            var cnr = new SearchConditionNormalizer();
            cnr.CollectConditions(querySpecification);
            var where = cnr.GenerateWhereClauseSpecificToTable(table);

            // Build table specific query
            var sql = new StringWriter();

            sql.Write("SELECT ");

            if (top > 0)
            {
                sql.Write("TOP {0} ", top);
            }

            // Now write the referenced columns
            var referencedcolumns = new HashSet<string>(Jhu.Graywulf.Schema.SqlServer.SqlServerSchemaManager.Comparer);

            int q = 0;
            if (includePrimaryKey)
            {
                var t = table.DatabaseObject as Jhu.Graywulf.Schema.Table;
                foreach (var cr in t.PrimaryKey.Columns.Values)
                {
                    var columnname = String.Format(
                        "{0}.{1}",
                        QuoteIdentifier(table.Alias),
                        QuoteIdentifier(cr.ColumnName));

                    if (!referencedcolumns.Contains(columnname))
                    {
                        if (q != 0)
                        {
                            sql.Write(", ");
                        }
                        sql.Write(columnname);
                        q++;

                        referencedcolumns.Add(columnname);
                    }
                }
            }


            foreach (var cr in table.ColumnReferences.Where(c => c.IsReferenced))
            {
                var columnname = GetResolvedColumnName(cr);     // TODO: verify

                if (!referencedcolumns.Contains(columnname))
                {
                    if (q != 0)
                    {
                        sql.Write(", ");
                    }
                    sql.Write(columnname);
                    q++;

                    referencedcolumns.Add(columnname);
                }
            }

            // From cluse
            sql.Write(" FROM {0} ", GetResolvedTableName(table));
            if (!String.IsNullOrWhiteSpace(table.Alias))
            {
                sql.Write("AS {0} ", QuoteIdentifier(table.Alias));
            }

            if (where != null)
            {
                Execute(sql, where);
            }

            return sql.ToString();
        }

        #endregion



        public string GenerateCreateTableQuery(Table table, bool primaryKey, bool indexes)
        {
            if (table.Columns.Count == 0)
            {
                throw new InvalidOperationException("The table doesn't have any columns.");     // TODO ***
            }

            var sql = new StringBuilder();

            sql.Append("CREATE TABLE ");
            sql.Append(GetResolvedTableName(table));
            sql.AppendLine(" (");

            var columns = GenerateColumnList(
                table.Columns.Values.OrderBy(ci => ci.ID),
                null,
                ColumnListType.CreateTable);

            sql.Append(columns);

            if (primaryKey && table.PrimaryKey != null)
            {
                // If primary is specified directly
                sql.AppendLine(",");

                var constraint = GeneratePrimaryKeyConstraint(table, table.PrimaryKey.Columns.Values.OrderBy(ci => ci.KeyOrdinal));

                sql.Append(constraint);
            }
            else if (primaryKey)
            {
                // If key columns are to be taken as primary key

                var keys = table.Columns.Values.Where(ci => ci.IsKey).OrderBy(ci => ci.ID).ToArray();

                if (keys.Length > 0)
                {
                    sql.AppendLine(",");

                    var constraint = GeneratePrimaryKeyConstraint(table, keys);

                    sql.Append(constraint);
                }
            }

            sql.AppendLine();
            sql.AppendLine(" )");

            // TODO: add index generation

            return sql.ToString();
        }

        private string GeneratePrimaryKeyConstraint(Table table, IEnumerable<Column> columns)
        {
            var sql = new StringBuilder();

            sql.Append("CONSTRAINT ");
            sql.Append(QuoteIdentifier(String.Format("PK_{0}_{1}", table.SchemaName, table.TableName)));
            sql.AppendLine(" PRIMARY KEY (");

            var collist = GenerateColumnList(
                    columns,
                    null,
                    ColumnListType.CreateIndex);

            sql.Append(collist);

            sql.AppendLine();
            sql.AppendLine(" )");

            return sql.ToString();
        }

        public string GenerateColumnList(IEnumerable<Column> columns, string tableAlias, ColumnListType type)
        {
            var columnlist = new StringBuilder();
            string format = null;

            switch (type)
            {
                case ColumnListType.CreateTable:
                    format = "[{1}] {2} {3}";
                    break;
                case ColumnListType.CreateIndex:
                    format = "[{1}] {4}";
                    break;
                case ColumnListType.CreateView:
                case ColumnListType.Insert:
                    format = "[{1}]";
                    break;
                case ColumnListType.Select:
                    format = "{0}[{1}]";
                    break;
                default:
                    throw new NotImplementedException();
            }

            foreach (var column in columns)
            {
                if (columnlist.Length != 0)
                {
                    columnlist.Append(", ");
                }

                var nullspec = "";
                var orderspec = "";

                if (type == ColumnListType.CreateTable)
                {
                    nullspec = column.DataType.IsNullable ? "NULL" : "NOT NULL";
                }

                if (type == ColumnListType.CreateIndex && column is IndexColumn)
                {
                    orderspec = ((IndexColumn)column).Ordering == IndexColumnOrdering.Descending ? "DESC" : "ASC";
                }

                columnlist.AppendFormat(format,
                                        tableAlias == null ? String.Empty : String.Format("[{0}].", tableAlias),
                                        column.Name,
                                        column.DataType.NameWithLength,
                                        nullspec,
                                        orderspec);
            }

            return columnlist.ToString();
        }
    }
}
