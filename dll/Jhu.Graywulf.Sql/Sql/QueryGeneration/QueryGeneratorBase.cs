using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.LogicalExpressions;
using Jhu.Graywulf.Sql.QueryRendering;

namespace Jhu.Graywulf.Sql.QueryGeneration
{
    public abstract class QueryGeneratorBase : CodeGenerator
    {
        #region Private members

        private QueryRendererBase renderer;

        #endregion
        #region Properties

        protected QueryRendererBase Renderer
        {
            get { return renderer; }
        }

        #endregion
        #region Constructors and initializers

        protected QueryGeneratorBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.renderer = CreateQueryRenderer();
        }

        #endregion

        public abstract QueryRendererBase CreateQueryRenderer();

        public abstract ColumnListGeneratorBase CreateColumnListGenerator();

        public ColumnListGeneratorBase CreateColumnListGenerator(TableReference table, ColumnContext columnContext, ColumnListType listType)
        {
            var cg = CreateColumnListGenerator();
            cg.ListType = listType;
            cg.Columns.AddRange(table.FilterColumnReferences(columnContext));
            return cg;
        }

        #region Identifier formatting functions

        public string EscapeIdentifier(string identifier)
        {
            return identifier.Replace(".", "_");
        }

        public string GenerateEscapedUniqueName(TableReference table)
        {
            if (table.TableContext.HasFlag(TableContext.Subquery) ||
                table.TableContext.HasFlag(TableContext.CommonTable) ||
                table.IsComputed)
            {
                // We consider a table alias unique within a query, although this is
                // not a requirement by SQL Server which support using the same alias
                // in subqueries.

                return EscapeIdentifier(table.Alias);
            }
            else
            {
                return GenerateEscapedUniqueName(table.DatabaseObject, table.Alias);
            }
        }

        public string GenerateEscapedUniqueName(DatabaseObject dbobj, string alias)
        {
            string res = String.Empty;

            if (!String.IsNullOrWhiteSpace(dbobj.DatasetName))
            {
                res += String.Format("{0}_", EscapeIdentifier(dbobj.DatasetName));
            }

            if (!String.IsNullOrWhiteSpace(dbobj.DatabaseName))
            {
                res += String.Format("{0}_", EscapeIdentifier(dbobj.DatabaseName));
            }

            if (!String.IsNullOrWhiteSpace(dbobj.SchemaName))
            {
                res += String.Format("{0}_", EscapeIdentifier(dbobj.SchemaName));
            }

            if (!String.IsNullOrWhiteSpace(dbobj.ObjectName))
            {
                res += String.Format("{0}", EscapeIdentifier(dbobj.ObjectName));
            }

            // If a table is referenced more than once we need an alias to distinguish them
            if (!String.IsNullOrWhiteSpace(alias))
            {
                res += String.Format("_{0}", EscapeIdentifier(alias));
            }

            return res;
        }

        public string GeneratePrimaryKeyName(DatabaseObject table)
        {
            return GeneratePrimaryKeyName(table.SchemaName, table.ObjectName);
        }

        public string GeneratePrimaryKeyName(string schemaName, string tableName)
        {
            return renderer.GetQuotedIdentifier(String.Format("PK_{0}_{1}", schemaName, tableName));
        }
                
        #endregion
        #region Complete query generators

        // These functions don't use the parsing tree, they generate certain
        // types of queries.

        public abstract string GenerateSelectStarQuery(TableReference tableReference, int top);

        public abstract string GenerateSelectStarQuery(TableReference tableReference, string orderBy, long from, long max);

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
        public abstract string GenerateSelectStarQuery(TableOrView tableOrView, int top);

        public abstract string GenerateSelectStarQuery(TableOrView tableOrView, string orderBy, long from, long max);

        protected abstract string GenerateTopExpression(int top);

        protected abstract string GenerateOffsetExpression(long from, long max);

        public abstract string GenerateMostRestrictiveTableQuery(string tableName, string tableAlias, string columnList, string where, int top);

        public virtual string GenerateCountStarQuery(string subquery)
        {
            // TODO: modify to parse query first into separate SELECTS
            // remove ending ; etc

            // Use count_big maybe?

            return String.Format("SELECT CAST(COUNT(*) AS bigint) FROM ({0}) __countstar", subquery);
        }

        #endregion
    }
}
