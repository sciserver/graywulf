using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlParser
{
    public class SqlNameResolver
    {
        private static readonly HashSet<string> SystemFunctionNames = new HashSet<string>(Schema.SchemaManager.Comparer)
        {
            "OPENDATASOURCE", "OPENQUERY", "OPENROWSET", "OPENXML",

            "AVG", "MIN", "CHECKSUM_AGG", "SUM", "COUNT", "STDEV", "COUNT_BIG", "STDEVP", "GROUPING", "VAR", "GROUPING_ID", "VARP", "MAX",

            "RANK", "NTILE", "DENSE_RANK", "ROW_NUMBER",
 
            "CAST", "CONVERT", "PARSE", "TRY_CAST", "TRY_CONVERT", "TRY_PARSE",

            "SYSDATETIME", "SYSDATETIMEOFFSET", "SYSUTCDATETIME", "CURRENT_TIMESTAMP", "GETDATE", "GETUTCDATE", "DATENAME", "DATEPART", "DAY", "MONTH", "YEAR",
            "DATEFROMPARTS", "DATETIME2FROMPARTS", "DATETIMEFROMPARTS", "DATETIMEOFFSETFROMPARTS", "SMALLDATETIMEFROMPARTS", "TIMEFROMPARTS",
            "DATEDIFF", "DATEADD", "EOMONTH", "SWITCHOFFSET", "TODATETIMEOFFSET",

            "CHOOSE", "IIF",

            "ABS", "DEGREES", "RAND", "ACOS", "EXP", "ROUND", "ASIN", "FLOOR", "SIGN", "ATAN", "LOG", "SIN", "ATN2", "LOG10", "SQRT", "CEILING",
            "PI", "SQUARE", "COS", "POWER", "TAN", "COT", "RADIANS",
 
            "INDEX_COL", "APP_NAME", "INDEXKEY_PROPERTY", "APPLOCK_MODE", "INDEXPROPERTY", "APPLOCK_TEST", "ASSEMBLYPROPERTY", "OBJECT_DEFINITION",
            "COL_LENGTH", "OBJECT_ID", "COL_NAME", "OBJECT_NAME", "COLUMNPROPERTY", "OBJECT_SCHEMA_NAME", "DATABASE_PRINCIPAL_ID", "OBJECTPROPERTY",
            "DATABASEPROPERTYEX", "OBJECTPROPERTYEX", "DB_ID", "ORIGINAL_DB_NAME", "DB_NAME", "PARSENAME", "FILE_ID", "SCHEMA_ID", "FILE_IDEX",
            "SCHEMA_NAME", "FILE_NAME", "SCOPE_IDENTITY", "FILEGROUP_ID", "SERVERPROPERTY", "FILEGROUP_NAME", "STATS_DATE", "FILEGROUPPROPERTY",
            "TYPE_ID", "FILEPROPERTY", "TYPE_NAME", "FULLTEXTCATALOGPROPERTY", "TYPEPROPERTY", "FULLTEXTSERVICEPROPERTY",
 
            "CERTENCODED", "PWDCOMPARE", "CERTPRIVATEKEY", "PWDENCRYPT", "CURRENT_USER", "SCHEMA_ID", "DATABASE_PRINCIPAL_ID", "SCHEMA_NAME", 
            "SESSION_USER", "SUSER_ID", "SUSER_SID", "HAS_PERMS_BY_NAME", "SUSER_SNAME", "IS_MEMBER", "SYSTEM_USER", "IS_ROLEMEMBER", "SUSER_NAME", 
            "IS_SRVROLEMEMBER", "USER_ID", "ORIGINAL_LOGIN", "USER_NAME", "PERMISSIONS",
 
            "ASCII", "LTRIM", "SOUNDEX", "CHAR", "NCHAR", "SPACE", "CHARINDEX", "PATINDEX", "STR", "CONCAT", "QUOTENAME", "STUFF", "DIFFERENCE",
            "REPLACE", "SUBSTRING", "FORMAT", "REPLICATE", "UNICODE", "LEFT", "REVERSE", "UPPER", "LEN", "RIGHT", "LOWER", "RTRIM", 
 
            "ERROR_SEVERITY", "ERROR_STATE", "FORMATMESSAGE", "GETANSINULL", "GET_FILESTREAM_TRANSACTION_CONTEXT", "HOST_ID", "BINARY_CHECKSUM",
            "HOST_NAME", "CHECKSUM", "ISNULL", "CONNECTIONPROPERTY", "ISNUMERIC", "CONTEXT_INFO", "MIN_ACTIVE_ROWVERSION", "CURRENT_REQUEST_ID",
            "NEWID", "ERROR_LINE", "NEWSEQUENTIALID", "ERROR_MESSAGE", "ROWCOUNT_BIG", "ERROR_NUMBER", "XACT_STATE", "ERROR_PROCEDURE",

            "PATINDEX", "TEXTVALID", "TEXTPTR",
        };

        #region Property storage variables

        private SchemaManager schemaManager;

        private string defaultTableDatasetName;
        private string defaultFunctionDatasetName;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the schema manager to be used by the name resolver
        /// </summary>
        public SchemaManager SchemaManager
        {
            get { return schemaManager; }
            set { schemaManager = value; }
        }

        /// <summary>
        /// Gets or sets the default dataset name to be assumed when no
        /// dataset is specified
        /// </summary>
        public string DefaultTableDatasetName
        {
            get { return defaultTableDatasetName; }
            set { defaultTableDatasetName = value; }
        }

        public string DefaultFunctionDatasetName
        {
            get { return defaultFunctionDatasetName; }
            set { defaultFunctionDatasetName = value; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Creates a new SqlNameResolver
        /// </summary>
        public SqlNameResolver()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Initializez member variables
        /// </summary>
        private void InitializeMembers()
        {
            this.schemaManager = null;

            this.defaultTableDatasetName = String.Empty;
            this.defaultFunctionDatasetName = String.Empty;
        }

        #endregion

        /// <summary>
        /// Executes the name resolution over a query
        /// </summary>
        /// <param name="selectStatement"></param>
        public void Execute(SelectStatement selectStatement)
        {
            ResolveSelectStatement(selectStatement, 0);
        }

        protected void ResolveSelectStatement(SelectStatement selectStatement, int depth)
        {
            // The SqlParser build the parsing tree and tags many nodes with TableReference and ColumnReference objects.
            // At this point these references only contain information directly available in the query, but names are
            // not verified against the database schema.

            // A query consists of a set of query specifications combined using set operators (UNION, EXCEPT etc.)
            // Each query specification has a FROM clause with a complex table expression. A table expression is a
            // set of table sources combined with join operators. A table source can be any of the following four:
            // a table (or view), a table-valued function, a table-valued variable or a subquery.
            // The WHERE clause main contain additional semi-join criteria which contain subqueries.

            // Steps of name resolution:

            // 1. Identify all query specifications and execute name resolution on each of them
            // 2. For each query specification, substitute default values if null is found
            // 3. Collect column descriptions from table sources
            // 4. Resolve column aliases
            // 5. Resolve table aliases
            // 6. Assign default column aliases

            SubstituteFunctionDefaults(selectStatement);
            ResolveFunctionReferences(selectStatement);

            var qe = selectStatement.QueryExpression;

            ResolveQueryExpression(qe, depth);

            var firstqs = qe.FindDescendant<QuerySpecification>();
            var orderby = selectStatement.OrderByClause;

            ResolveOrderByClause(orderby, firstqs);
        }

        protected void ResolveQueryExpression(QueryExpression qe, int depth)
        {
            // Resolve query specifications in the FROM clause
            foreach (var qs in qe.EnumerateDescendants<QuerySpecification>())
            {
                ResolveQuerySpecification(qs, depth);
            }

            // Copy select list columns from the very first query specification
            var firstqs = qe.FindDescendant<QuerySpecification>();
            qe.TableReference.ColumnReferences.AddRange(firstqs.ResultsTableReference.ColumnReferences);
        }

        /// <summary>
        /// Internal routine to perform the name resolution steps on a single
        /// query specification
        /// </summary>
        /// <param name="qs"></param>
        protected void ResolveQuerySpecification(QuerySpecification qs, int depth)
        {
            // At this point the table and column references are all parsed
            // from the query but no name resolution and cross-identification
            // of these references have happened yet. After the cross-idenfication
            // routine, the same tables and columns will be tagged by the
            // same TableReference and ColumnReference instances.

            // First of all, call everything recursively for subqueries. Subqueries
            // can appear within the table sources and in the where clause semi-join
            // expressions
            foreach (var sq in qs.EnumerateSubqueries())
            {
                ResolveSelectStatement(sq.SelectStatement, depth + 1);
            }

            // Substitute default dataset names and schema names
            // This is typically the MYDB and dbo
            SubstituteTableAndColumnDefaults(qs);

            // Column references will be stored under the query specification
            CollectSourceTableReferences(qs);

            // Column identifiers can contain table names, aliases or nothing,
            // resolve them now
            ResolveTableReferences(qs);

            // Substitute SELECT * expressions
            SubstituteStars(qs);

            // Resolve column references of each occurance
            ResolveColumnReferences(qs);

            // Copy resultset columns to the appropriate collection
            CopyResultsColumns(qs);

            // Add default aliases to column expressions in the form of tablealias_columnname
            if (depth == 0)
            {
                AssignDefaultColumnAliases(qs);
            }
        }

        protected void ResolveOrderByClause(OrderByClause orderBy, QuerySpecification firstqs)
        {
            if (orderBy != null)
            {
                ResolveTableReferences(firstqs, orderBy);
                ResolveColumnReferences(firstqs, orderBy, ColumnContext.OrderBy);
            }
        }

        /// <summary>
        /// Substitutes dataset and schema defaults into table source table references
        /// </summary>
        /// <param name="qs"></param>
        protected void SubstituteTableAndColumnDefaults(QuerySpecification qs)
        {
            foreach (var tr in qs.EnumerateSourceTableReferences(false))
            {
                if (tr.IsTableOrView)
                {
                    tr.SubstituteDefaults(SchemaManager, defaultTableDatasetName);
                }
                else if (tr.IsUdf)
                {
                    tr.SubstituteDefaults(SchemaManager, defaultFunctionDatasetName);
                }
            }
        }

        protected void SubstituteFunctionDefaults(Node node)
        {
            foreach (var fi in node.EnumerateDescendantsRecursive<FunctionIdentifier>())
            {
                fi.FunctionReference.SubstituteDefaults(SchemaManager, defaultFunctionDatasetName);
            }
        }

        /// <summary>
        /// Collect list of table sources and load columns from the schema.
        /// </summary>
        /// <remarks>
        /// Source tables are put into a dictionary that is keyed by table alias
        /// or table name.
        /// </remarks>
        /// <param name="qs"></param>
        private void CollectSourceTableReferences(QuerySpecification qs)
        {
            // --- Collect column references from subqueries or load from the database schema

            foreach (var tr in qs.EnumerateSourceTableReferences(false))
            {
                string tablekey;
                if (tr.IsSubquery || tr.IsComputed || tr.IsUdf || tr.Alias != null)
                {
                    tablekey = tr.Alias;
                }
                else
                {
                    // If no alias is used then use table name
                    tablekey = tr.DatabaseObjectName;
                }

                // Make sure that table key is used only once
                if (qs.SourceTableReferences.ContainsKey(tablekey))
                {
                    throw CreateException(ExceptionMessages.DuplicateTableAlias, null, tablekey, tr.Node);
                }
                else
                {
                    var ntr = new TableReference(tr);

                    if (!ntr.IsSubquery && !ntr.IsComputed)
                    {
                        // Load table description from underlying schema
                        // Attempt to load dataset and throw exception of name cannot be resolved
                        DatasetBase ds;

                        try
                        {
                            ds = schemaManager.Datasets[ntr.DatasetName];
                        }
                        catch (KeyNotFoundException ex)
                        {
                            throw CreateException(ExceptionMessages.UnresolvableDatasetReference, ex, ntr.DatasetName, ntr.Node);
                        }
                        catch (SchemaException ex)
                        {
                            throw CreateException(ExceptionMessages.UnresolvableDatasetReference, ex, ntr.DatasetName, ntr.Node);
                        }

                        ntr.DatabaseObject = ds.GetObject(ntr.DatabaseName, ntr.SchemaName, ntr.DatabaseObjectName);

                        // Load column descriptions for the table
                        ntr.LoadColumnReferences(schemaManager);
                    }

                    qs.SourceTableReferences.Add(tablekey, ntr);
                }
            }
        }

        /// <summary>
        /// Resolves the table references of all nodes below a query specification
        /// not descending into subqueries
        /// </summary>
        /// <param name="qs"></param>
        private void ResolveTableReferences(QuerySpecification qs)
        {
            ResolveTableReferences(qs, (Node)qs);
        }

        /// <summary>
        /// Resolves all table references of all nodes below a node,
        /// not descending into subqueries
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="n"></param>
        private void ResolveTableReferences(QuerySpecification qs, Node n)
        {
            foreach (object o in n.Nodes)
            {
                // Skip the into and clause and subqueries
                if (!(o is IntoClause) && !(o is SubqueryTableSource))
                {
                    if (o is Node)
                    {
                        ResolveTableReferences(qs, (Node)o);   // Recursive call
                    }
                }
            }

            if (n is ITableReference && ((ITableReference)n).TableReference != null)
            {
                ResolveTableReference(qs, (ITableReference)n);
            }
        }

        /// <summary>
        /// Resolves a table reference to a table listed in SourceTableReferences
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="tr"></param>
        private void ResolveTableReference(QuerySpecification qs, ITableReference node)
        {
            // Try to resolve the table alias part of a table reference
            // If and alias or table name is specified, this can be done based on
            // the already collected table sources.
            // If no table or alias is specified and the current node is a column reference,
            // where the column is not a complex expression, resolution might be successful by
            // column name only.

            if (!node.TableReference.IsUndefined)
            {
                TableReference ntr = null;
                string alias = null;

                if (node.TableReference.Alias != null)
                {
                    // if table alias found explicitly
                    alias = node.TableReference.Alias;
                }
                else if (node.TableReference.DatasetName == null &&
                        node.TableReference.DatabaseName == null &&
                        node.TableReference.SchemaName == null &&
                        node.TableReference.DatabaseObjectName != null &&
                        qs.SourceTableReferences.ContainsKey(node.TableReference.DatabaseObjectName))
                {
                    // if only table name found and that's an alias
                    alias = node.TableReference.DatabaseObjectName;
                }

                if (alias != null)
                {
                    ntr = qs.SourceTableReferences[alias];
                }
                else
                {
                    // Check if dataset specified and make sure it's valid
                    if (node.TableReference.DatasetName != null)
                    {
                        if (!schemaManager.Datasets.ContainsKey(node.TableReference.DatasetName))
                        {
                            throw CreateException(ExceptionMessages.UnresolvableDatasetReference, null, node.TableReference.DatasetName, (Node)node);
                        }
                    }

                    // if only a table name found and that's not an alias -> must be a table
                    int q = 0;
                    foreach (var key in qs.SourceTableReferences.Keys)
                    {
                        var tr = qs.SourceTableReferences[key];

                        if (tr.Compare(node.TableReference))
                        {
                            if (q != 0)
                            {
                                throw CreateException(ExceptionMessages.AmbigousTableReference, null, node.TableReference.DatabaseObjectName, (Node)node);
                            }

                            ntr = tr;
                            q++;
                        }
                    }
                }

                if (ntr == null)
                {
                    throw CreateException(ExceptionMessages.UnresolvableTableReference, null, node.TableReference.DatabaseObjectName, (Node)node);
                }

                node.TableReference = ntr;
            }
        }

        private void ResolveColumnReferences(QuerySpecification qs)
        {
            ResolveColumnReferences(qs, (Node)qs, ColumnContext.None);
        }

        /// <summary>
        /// Resolves all table references of all nodes below a node,
        /// not descending into subqueries
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="n"></param>
        private void ResolveColumnReferences(QuerySpecification qs, Node n, ColumnContext context)
        {
            context = GetColumnContext(n, context);

            foreach (object o in n.Nodes)
            {
                // Skip the into and clause and subqueries
                if (!(o is IntoClause) && !(o is SubqueryTableSource))
                {
                    if (o is Node)
                    {
                        ResolveColumnReferences(qs, (Node)o, context);   // Recursive call
                    }
                }
            }

            if (n is IColumnReference)
            {
                ResolveColumnReference(qs, (IColumnReference)n, context);
            }
        }

        private void ResolveColumnReference(QuerySpecification qs, IColumnReference cr, ColumnContext context)
        {
            // Try to resolve the table belonging to a column based solely on
            // column name. This function is called only on column references with
            // unspecified table parts.
            // Star columns cannot be resolved, treat them separately

            if (!cr.ColumnReference.IsStar && !cr.ColumnReference.IsComplexExpression)
            {

                ColumnReference ncr = null;
                int q = 0;

                if (cr.ColumnReference.TableReference.IsUndefined)
                {
                    // This has an empty table reference (only column name specified)
                    // Look for a match based on column name only
                    foreach (var tr in qs.SourceTableReferences.Values)
                    {
                        foreach (var ccr in tr.ColumnReferences)
                        {
                            if (cr.ColumnReference.Compare(ccr))
                            {
                                if (q != 0)
                                {
                                    throw CreateException(ExceptionMessages.AmbigousColumnReference, null, cr.ColumnReference.ColumnName, (Node)cr);
                                }

                                ncr = ccr;
                                q++;
                            }
                        }
                    }
                }
                else if (!cr.ColumnReference.TableReference.IsUndefined)
                {
                    foreach (var ccr in cr.ColumnReference.TableReference.ColumnReferences)
                    {
                        if (cr.ColumnReference.Compare(ccr))
                        {
                            if (q != 0)
                            {
                                throw CreateException(ExceptionMessages.AmbigousColumnReference, null, cr.ColumnReference.ColumnName, (Node)cr);
                            }

                            ncr = ccr;
                            q++;
                        }
                    }
                }

                if (q == 0)
                {
                    throw CreateException(ExceptionMessages.UnresolvableColumnReference, null, cr.ColumnReference.ColumnName, (Node)cr);
                }

                // Make copy here and preserve alias!
                ncr.ColumnContext |= context;

                ncr = new ColumnReference(ncr);
                if (cr.ColumnReference != null && cr.ColumnReference.ColumnAlias != null)
                {
                    ncr.ColumnAlias = cr.ColumnReference.ColumnAlias;
                }
                cr.ColumnReference = ncr;
            }
        }

        protected virtual ColumnContext GetColumnContext(Node n, ColumnContext context)
        {
            if (n is SelectList)
            {
                context = ColumnContext.SelectList;
            }
            else if (n is FromClause)
            {
                context = ColumnContext.From;
            }
            else if (n is WhereClause)
            {
                context = ColumnContext.Where;
            }
            else if (n is GroupByClause)
            {
                context = ColumnContext.GroupBy;
            }
            else if (n is HavingClause)
            {
                context = ColumnContext.Having;
            }

            return context;
        }

        /// <summary>
        /// Replace SELECT * and SELECT alias.* with explicit column lists
        /// </summary>
        /// <param name="qs"></param>
        private void SubstituteStars(QuerySpecification qs)
        {
            SubstituteStars(qs, qs);
        }

        private void SubstituteStars(QuerySpecification qs, Node node)
        {
            if (node != null)
            {
                var n = node.Stack.First;

                while (n != null)
                {
                    if (n.Value is SelectList)
                    {
                        n.Value = ((SelectList)n.Value).SubstituteStars();
                    }

                    n = n.Next;
                }
            }
        }

        protected void ResolveFunctionReferences(Node n)
        {
            foreach (object o in n.Nodes)
            {
                // Skip the into and clause and subqueries
                if (!(o is IntoClause))
                {
                    if (o is Node)
                    {
                        ResolveFunctionReferences((Node)o);   // Recursive call
                    }
                }
            }

            if (n is IFunctionReference && ((IFunctionReference)n).FunctionReference != null)
            {
                ResolveFunctionReference((IFunctionReference)n);
            }
        }

        protected void ResolveFunctionReference(IFunctionReference node)
        {
            // *** TODO: handle sys function here
            if (!node.FunctionReference.IsUdf)
            {
                if (!IsSystemFunctionName(node.FunctionReference.SystemFunctionName))
                {
                    throw CreateException(ExceptionMessages.UnknownFunctionName, null, node.FunctionReference.SystemFunctionName, (Node)node);
                }
            }
            else
            {
                // Check if dataset specified and make sure it's valid
                if (node.FunctionReference.DatasetName != null)
                {
                    if (!schemaManager.Datasets.ContainsKey(node.FunctionReference.DatasetName))
                    {
                        throw CreateException(ExceptionMessages.UnresolvableDatasetReference, null, node.FunctionReference.DatasetName, (Node)node);
                    }
                }

                var ds = schemaManager.Datasets[node.FunctionReference.DatasetName];

                node.FunctionReference.DatabaseObject = ds.GetObject(node.FunctionReference.DatabaseName, node.FunctionReference.SchemaName, node.FunctionReference.DatabaseObjectName);
            }
        }

        protected virtual bool IsSystemFunctionName(string name)
        {
            return SystemFunctionNames.Contains(name);
        }

        /// <summary>
        /// Adds default aliases to columns with no aliases specified in the query
        /// </summary>
        /// <param name="qs"></param>
        protected void AssignDefaultColumnAliases(QuerySpecification qs)
        {
            var aliases = new HashSet<string>(SchemaManager.Comparer);

            foreach (var ce in qs.EnumerateSelectListColumnExpressions())
            {
                var cr = ce.ColumnReference;

                string alias;

                if (cr.ColumnAlias == null)
                {
                    if (cr.ColumnName == null)
                    {
                        alias = GetUniqueColumnAlias(aliases, String.Format("Col_{0}", cr.SelectListIndex));
                    }
                    else
                    {
                        if (cr.TableReference != null && cr.TableReference.Alias != null)
                        {
                            alias = GetUniqueColumnAlias(aliases, String.Format("{0}_{1}", cr.TableReference.Alias, cr.ColumnName));
                        }
                        else
                        {
                            alias = GetUniqueColumnAlias(aliases, cr.ColumnName);
                        }
                    }
                }
                else
                {
                    // Alias is set explicitly, so do not make it unique forcibly
                    alias = cr.ColumnAlias;
                }

                aliases.Add(alias);
                cr.ColumnAlias = alias;
            }
        }

        private string GetUniqueColumnAlias(HashSet<string> aliases, string alias)
        {
            int q = 0;
            var alias2 = alias;
            while (aliases.Contains(alias2))
            {
                alias2 = String.Format("{0}_{1}", alias, q);
                q++;
            }

            return alias2;
        }

        private void CopyResultsColumns(QuerySpecification qs)
        {
            int index = 0;

            foreach (var ce in qs.EnumerateSelectListColumnExpressions())
            {
                var cr = ce.ColumnReference;

                cr.SelectListIndex = index++;
                qs.ResultsTableReference.ColumnReferences.Add(cr);
            }
        }

        /// <summary>
        /// Creates and parameterizes and exception to be thrown by the name resolver.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="objectName"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        protected Exception CreateException(string message, Exception innerException, string objectName, Node node)
        {
            string msg;
            var id = node.FindDescendantRecursive<Identifier>();

            if (id != null)
            {
                msg = String.Format(message, objectName, id.Line + 1, id.Col + 1);
            }
            else
            {
                msg = String.Format(message, objectName, "?", "?");
            }

            NameResolverException ex = new NameResolverException(msg, innerException);
            ex.Token = id;

            return ex;
        }

        /// <summary>
        /// Creates and parameterizes and exception to be thrown by the name resolver.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected Exception CreateException(string message, Token token)
        {
            string msg;

            msg = String.Format(message, null, token.Line + 1, token.Pos + 1);

            NameResolverException ex = new NameResolverException(msg);
            ex.Token = token;

            return ex;
        }
    }
}
