using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser.SqlCodeGen
{
    public class SqlServerCodeGenerator : SqlCodeGeneratorBase
    {
        public SqlServerCodeGenerator()
        {
        }

        public static string GetCode(Node node, bool resolvedNames)
        {
            var sw = new StringWriter();
            var cg = new SqlServerCodeGenerator();
            cg.ResolveNames = resolvedNames;
            cg.Execute(sw, node);
            return sw.ToString();
        }

        public override bool WriteColumnExpression(ColumnExpression node)
        {
            if (ResolveNames)
            {
                Expression ex = node.FindDescendant<Expression>();
                WriteNode(ex);
                if (!node.ColumnReference.IsStar && !String.IsNullOrEmpty(node.ColumnReference.ColumnAlias))
                {
                    Writer.Write(" AS [{0}]", node.ColumnReference.ColumnAlias);
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool WriteColumnIdentifier(ColumnIdentifier node)
        {
            if (ResolveNames)
            {
                Writer.Write(node.ColumnReference.GetFullyResolvedName());
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool WriteTableAlias(TableAlias node)
        {
            /*if (ResolveAliases)
            {
                return false;
            }*/
            
            if (ResolveNames)
            {
                Writer.Write("[{0}]", Util.RemoveIdentifierQuotes(node.Value));
                return false;
            }
            else
            {
                Writer.Write(node.Value);
                return false;
            }
        }

        public override bool WriteTableOrViewName(TableOrViewName node)
        {
            if (ResolveNames)
            {
                Writer.Write(GetResolvedTableName(node.TableReference));
                return false;
            }
            else
            {
                return true;
            }
        }

        protected override string QuoteIdentifier(string identifier)
        {
            return String.Format("[{0}]", identifier);
        }

        // ---

        private string GetResolvedTableName(TableReference table)
        {
            if (table.IsUdf || table.IsSubquery || table.IsComputed)
            {
                return QuoteIdentifier(table.Alias);
            }
            else
            {
                // If it is linked up to the schema, return
                if (table.DatabaseObject != null)
                {
                    return table.DatabaseObject.GetFullyResolvedName();
                }
                else
                {
                    string res = String.Empty;

                    // If it's not resolved yet
                    if (table.DatabaseName != null) res += QuoteIdentifier(table.DatabaseName) + ".";
                    if (table.SchemaName != null) res += QuoteIdentifier(table.SchemaName) + ".";
                    if (table.DatabaseObjectName != null) res += QuoteIdentifier(table.DatabaseObjectName);

                    return res;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkedServerName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        /// <remarks>This is used by the web interface's 'peek' function</remarks>
        public override string GenerateTableSelectStarQuery(string linkedServerName, string databaseName, string schemaName, string tableName, int top)
        {
            string sql = "SELECT {0} t.* FROM {1}{2}[{3}].[{4}] AS t";

            string topstr = String.Empty;
            if (top != 0)
            {
                topstr = String.Format("TOP {0}", top);
            }

            if (!String.IsNullOrWhiteSpace(linkedServerName))
            {
                linkedServerName = String.Format("[{0}].", linkedServerName);
            }

            if (!String.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = String.Format("[{0}].", databaseName);
            }

            return String.Format(sql, topstr, linkedServerName, databaseName, schemaName, tableName);
        }


        public override string GenerateTableSelectStarQuery(string linkedServerName, TableReference table, int top)
        {
            var sql = "SELECT {0} {1}.* FROM {2}";

            // top string
            var topstr = String.Empty;
            if (top != 0)
            {
                topstr = String.Format("TOP {0}", top);
            }

            // build table name
            if (!String.IsNullOrWhiteSpace(linkedServerName))
            {
                linkedServerName = String.Format("[{0}].", linkedServerName);
            }
            var tablename = String.Format(
                "{0}{1} {3}",
                linkedServerName,
                table.FullyQualifiedName,
                table.Alias);


            return String.Format(sql, topstr, tablename);
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
                    var columnname = String.Format("[{0}].[{1}]", table.Alias, cr.ColumnName);

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
                var columnname = cr.GetFullyResolvedName();

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
            sql.Write(" FROM {0}", table.GetFullyResolvedName());
            if (!String.IsNullOrWhiteSpace(table.Alias))
            {
                sql.Write(" AS [{0}]", table.Alias);
            }

            sql.Write(" ");

            if (where != null)
            {
                Execute(sql, where);
            }

            return sql.ToString();
        }

        public string GenerateTableStatisticsQuery(TableReference table)
        {
            if (table.Statistics == null)
            {
                throw new InvalidOperationException();
            }

            // Build table specific where clause
            var cnr = new SearchConditionNormalizer();
            cnr.NormalizeQuerySpecification(((TableSource)table.Node).QuerySpecification);
            var wh = cnr.GenerateWhereClauseSpecificToTable(table);

            var where = new StringWriter();
            if (wh != null)
            {
                var cg = new SqlServerCodeGenerator();
                cg.Execute(where, wh);
            };

            //*** TODO: move into resource
            string sql = String.Format(@"
IF OBJECT_ID('tempdb..##keys_{4}') IS NOT NULL
DROP TABLE ##keys_{4}

SELECT CAST({2} AS float) AS __key
INTO ##keys_{4}
FROM {0} {1}
{3};

DECLARE @count bigint = @@ROWCOUNT;
DECLARE @step bigint = @count / @bincount;

IF (@step = 0) SET @step = NULL;

WITH q AS
(
	SELECT __key, ROW_NUMBER() OVER (ORDER BY __key) __rn
	FROM ##keys_{4}
)
SELECT __key, __rn
FROM q
WHERE __rn % @step = 1 OR __rn = @count;

DROP TABLE ##keys_{4};
",
         GetResolvedTableName(table),      // TODO: Does it return database name???
         table.Alias == null ? "" : String.Format(" AS [{0}] ", table.Alias),
         table.Statistics.KeyColumn,
         where.ToString(),
         Guid.NewGuid().ToString().Replace('-', '_'));

            return sql;
        }
    }
}
