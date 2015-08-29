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

        public override SqlColumnListGeneratorBase CreateColumnListGenerator(TableReference table, ColumnContext columnContext, ColumnListType listType)
        {
            return new SqlServerColumnListGenerator(table, columnContext, listType);
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

            var columnlist = CreateColumnListGenerator(table, columnContext, ColumnListType.ForSelectWithOriginalNameNoAlias);
            columnlist.TableAlias = null;

            // Build table specific query
            var sql = new StringBuilder();

            sql.Append("SELECT ");

            if (top > 0)
            {
                sql.AppendFormat("TOP {0} ", top);
            }

            sql.AppendLine();
            sql.AppendLine(columnlist.GetColumnListString());
            sql.AppendFormat("FROM {0} ", GetResolvedTableName(table));

            if (!String.IsNullOrWhiteSpace(table.Alias))
            {
                sql.AppendFormat("AS {0} ", GetQuotedIdentifier(table.Alias));
            }

            sql.AppendLine();
            sql.AppendLine(Execute(where));

            return sql.ToString();

            /* TODO: delete
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
                        GetQuotedIdentifier(table.Alias),
                        GetQuotedIdentifier(cr.ColumnName));

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
            }*/
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

            var columns = CreateColumnListGenerator(table, ColumnContext.All, ColumnListType.ForCreateTable);

            var sql = new StringBuilder();

            sql.Append("CREATE TABLE ");
            sql.Append(GetResolvedTableName(table));
            sql.AppendLine(" (");

            sql.Append(columns.GetColumnListString());

            if (primaryKey && table.TableOrView.PrimaryKey != null)
            {
                // If primary is specified directly
                sql.AppendLine(",");

                var constraint = GeneratePrimaryKeyConstraint(table);

                sql.Append(constraint);
            }
            else if (primaryKey)
            {
                throw new NotImplementedException();

                // TODO: Do we need this?
                /*
                // If key columns are to be taken as primary key

                var keys = table.Columns.Values.Where(ci => ci.IsKey).OrderBy(ci => ci.ID).ToArray();

                if (keys.Length > 0)
                {
                    sql.AppendLine(",");

                    var constraint = GeneratePrimaryKeyConstraint(table, keys);

                    sql.Append(constraint);
                }*/
            }

            sql.AppendLine();
            sql.AppendLine(" )");

            // TODO: add index generation

            return sql.ToString();
        }

        private string GeneratePrimaryKeyConstraint(TableReference table)
        {
            var columnlist = CreateColumnListGenerator(table, ColumnContext.PrimaryKey, ColumnListType.ForCreateIndex);

            var sql = new StringBuilder();

            sql.Append("CONSTRAINT ");
            sql.Append(GetQuotedIdentifier(String.Format("PK_{0}_{1}", table.SchemaName, table.DatabaseObjectName)));
            sql.AppendLine(" PRIMARY KEY (");

            sql.Append(columnlist.GetColumnListString());

            sql.AppendLine();
            sql.AppendLine(" )");

            return sql.ToString();
        }
    }
}
