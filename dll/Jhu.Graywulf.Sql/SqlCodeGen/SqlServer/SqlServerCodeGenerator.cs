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

        public SqlColumnListGeneratorBase CreateColumnListGenerator()
        {
            return new SqlServerColumnListGenerator();
        }

        public override SqlColumnListGeneratorBase CreateColumnListGenerator(TableReference table, ColumnContext columnContext, ColumnListType listType)
        {
            var cl = new SqlServerColumnListGenerator(table.FilterColumnReferences(columnContext))
            {
                ListType = listType,
            };

            return cl;
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

        public string GenerateCreateTableQuery(TableReference table, bool primaryKey, bool indexes)
        {
            if (table.DatabaseObject == null)
            {
                throw new ArgumentNullException("The table reference is not resolved");         // TODO ***
            }

            if (table.TableOrView.Columns.Count == 0)
            {
                throw new InvalidOperationException("The table doesn't have any columns.");     // TODO ***
            }

            var columns = CreateColumnListGenerator(table, ColumnContext.All, ColumnListType.CreateTableWithOriginalName);

            var sql = new StringBuilder();

            sql.Append("CREATE TABLE ");
            sql.Append(GetResolvedTableName(table));
            sql.AppendLine(" (");

            sql.Append(columns.Execute());

            if (primaryKey && table.TableOrView.PrimaryKey != null)
            {
                // If primary is specified directly
                var constraint = GeneratePrimaryKeyConstraint(table, ColumnContext.PrimaryKey);
                sql.AppendLine(",");
                sql.Append(constraint);
            }
            else if (primaryKey && table.TableOrView.Columns.Values.Count(i => i.IsKey) > 0)
            {
                // If key columns are to be taken as primary key
                var constraint = GeneratePrimaryKeyConstraint(table, ColumnContext.Key);
                sql.AppendLine(",");
                sql.Append(constraint);
            }

            sql.AppendLine();
            sql.AppendLine(" )");

            // TODO: add index generation

            return sql.ToString();
        }

        private string GeneratePrimaryKeyConstraint(TableReference table, ColumnContext columnContext)
        {
            var columnlist = CreateColumnListGenerator(table, columnContext, ColumnListType.CreateIndex);

            var sql = new StringBuilder();

            sql.Append("CONSTRAINT ");
            sql.Append(GetQuotedIdentifier(String.Format("PK_{0}_{1}", table.SchemaName, table.DatabaseObjectName)));
            sql.AppendLine(" PRIMARY KEY (");

            sql.Append(columnlist.Execute());

            sql.AppendLine();
            sql.AppendLine(" )");

            return sql.ToString();
        }

        public string GenerateCreatePrimaryKeyQuery(TableReference table)
        {
            if (table.DatabaseObject == null)
            {
                throw new ArgumentNullException("The table reference is not resolved");         // TODO ***
            }

            if (table.TableOrView.Columns.Count == 0)
            {
                throw new InvalidOperationException("The table doesn't have any columns.");     // TODO ***
            }

            var sql = new StringBuilder();
            var pk = GetQuotedIdentifier(String.Format("PK_{0}_{1}", table.SchemaName, table.DatabaseObjectName));

            sql.AppendFormat("ALTER TABLE {0}", GetResolvedTableName(table));
            sql.AppendLine();
            sql.Append("ADD ");
            sql.Append(GeneratePrimaryKeyConstraint(table, ColumnContext.PrimaryKey));

            return sql.ToString();
        }
    }
}
