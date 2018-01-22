using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class SqlNameResolver
    {
        #region Constants 
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

        private static readonly HashSet<string> SystemVariableNames = new HashSet<string>(Schema.SchemaManager.Comparer)
        {
            "ERROR", "IDENTITY", "ROWCOUNT", "FETCH_STATUS",

            "CONNECTION", "CPU_BUSY", "IDLE", "IO_BUSY", "PACK_SENT", "PACK_RECEIVED", "PACKET_ERRORS",
            "TIMETICKS", "TOTAL_ERRORS", "TOTAL_READ", "TOTAL_WRITER",
            "TRANCOUNT",
            "CURSOR_ROWS", "DATEFIRST", "DBTS", "DEF_SORTORDER_ID", "DEFAULT_LANGID",
            "FETCH_STATUS", "LANGID", "LANGUAGE", "LOCK_TIMEOUT", "MAX_CONNECTION",
            "MAX_PRECISION", "MICROSOFTVERSION", "NESTLEVEL", "OPTIONS",
            "PROCID", "REMSERVER", "SERVERNAME", "SERVICENAME", "SPID",
            "TEXTSIZE", "VERSION"
        };

        #endregion
        #region Property storage variables

        // The schema manager is used to resolve identifiers that are not local to the details,
        // i.e. database, table, columns etc. names
        private SchemaManager schemaManager;

        private string defaultTableDatasetName;
        private string defaultFunctionDatasetName;
        private string defaultOutputDatasetName;

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

        public string DefaultOutputDatasetName
        {
            get { return defaultOutputDatasetName; }
            set { defaultOutputDatasetName = value; }
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
        #region Statements
        
        /// <summary>
        /// Executes the name resolution over a query
        /// </summary>
        /// <param name="selectStatement"></param>
        public QueryDetails Execute(StatementBlock statementBlock)
        {
            // SqlParser builds the parsing tree and tags many nodes with TableReference and ColumnReference objects.
            // At this point these references only contain information directly available from the query, but names are
            // not verified against the database schema.

            // A query consists of a set of query specifications combined using set operators (UNION, EXCEPT etc.)
            // Each query specification has a FROM clause with a complex table expression. A table expression is a
            // set of table sources combined with join operators. A table source can be any of the following four:
            // a table (or view), a table-valued function, a table-valued variable or a subquery.
            // The WHERE clause may contain additional semi-join criteria which contain subqueries.
            // Furthermore, certain extensions to the SQL grammar may contain one or more table sources.

            // Steps of name resolution:

            // 1. Identify all query specifications and execute name resolution on each of them
            // 2. For each query specification, substitute default values of database name, schema, etc.
            //    if null is found
            // 3. Collect column descriptions from table sources
            // 4. Resolve column aliases
            // 5. Resolve table aliases
            // 6. Assign default column aliases

            var details = new QueryDetails();
            details.ParsingTree = statementBlock;
            Execute(details);
            return details;
        }

        public void Execute(QueryDetails details)
        {
            ResolveStatementBlock(details, details.ParsingTree);
            details.IsResolved = true;
        }

        protected void ResolveStatementBlock(QueryDetails details, StatementBlock statementBlock)
        {
            foreach (var statement in statementBlock.EnumerateDescendants<Statement>(true))
            {
                ResolveStatement(details, statement);
            }
        }

        private void ResolveStatement(QueryDetails details, Statement statement)
        {
            var s = statement.SpecificStatement;

            // Resolve current statement
            if (s.IsResolvable)
            {
                switch (s)
                {
                    case WhileStatement ss:
                        ResolveWhileStatement(details, ss);
                        break;
                    case ReturnStatement ss:
                        ResolveReturnStatement(details, ss);
                        break;
                    case IfStatement ss:
                        ResolveIfStatement(details, ss);
                        break;
                    case ThrowStatement ss:
                        ResolveThrowStatement(details, ss);
                        break;
                    case DeclareVariableStatement ss:
                        ResolveDeclareVariableStatement(details, ss);
                        break;
                    case DeclareTableStatement ss:
                        ResolveDeclareTableStatement(details, ss);
                        break;
                    case DeclareCursorStatement ss:
                        ResolveDeclareCursorStatement(details, ss);
                        break;
                    case SetCursorStatement ss:
                        ResolveSetCursorStatement(details, ss);
                        break;
                    case CursorOperationStatement ss:
                        ResolveCursorOperationStatement(details, ss);
                        break;
                    case FetchStatement ss:
                        ResolveFetchStatement(details, ss);
                        break;
                    case SetVariableStatement ss:
                        ResolveSetVariableStatement(details, ss);
                        break;
                    case CreateTableStatement ss:
                        ResolveCreateTableStatement(details, ss);
                        break;
                    case DropTableStatement ss:
                        ResolveDropTableStatement(details, ss);
                        break;
                    case TruncateTableStatement ss:
                        ResolveTruncateTableStatement(details, ss);
                        break;
                    case CreateIndexStatement ss:
                        ResolveCreateIndexStatement(details, ss);
                        break;
                    case DropIndexStatement ss:
                        ResolveDropIndexStatement(details, ss);
                        break;
                    case SelectStatement ss:
                        ResolveSelectStatement(details, ss);
                        break;
                    case InsertStatement ss:
                        ResolveInsertStatement(details, ss);
                        break;
                    case DeleteStatement ss:
                        ResolveDeleteStatement(details, ss);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            // Call recursively for sub-statements
            foreach (var ss in s.EnumerateSubStatements())
            {
                ResolveStatement(details, ss);
            }
        }

        private void ResolveWhileStatement(QueryDetails details, WhileStatement statement)
        {
            ResolveSubtree(details, statement.Condition);
        }

        private void ResolveReturnStatement(QueryDetails details, ReturnStatement statement)
        {
            // it might have a query in the parameter
            // do we support functions or stored procedures?
            throw new NotImplementedException();
        }

        private void ResolveIfStatement(QueryDetails details, IfStatement statement)
        {
            ResolveSubtree(details, statement.Condition);
        }

        private void ResolveThrowStatement(QueryDetails details, ThrowStatement statement)
        {
            // Resolve variables
            throw new NotImplementedException();
        }

        private void ResolveDeclareCursorStatement(QueryDetails details, DeclareCursorStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveSetCursorStatement(QueryDetails details, SetCursorStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveCursorOperationStatement(QueryDetails details, CursorOperationStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveFetchStatement(QueryDetails details, FetchStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveDeclareVariableStatement(QueryDetails details, DeclareVariableStatement statement)
        {
            foreach (var vd in statement.EnumerateDescendantsRecursive<VariableDeclaration>())
            {
                ResolveVariableDeclaration(details, vd);
            }
        }

        private void ResolveVariableDeclaration(QueryDetails details, VariableDeclaration vd)
        {
            var exp = vd.Expression;

            if (exp != null)
            {
                ResolveSubtree(details, exp);
            }

            if (!details.VariableReferences.ContainsKey(vd.VariableReference.Name))
            {
                details.VariableReferences.Add(vd.VariableReference.Name, vd.VariableReference);
            }
            else
            {
                throw NameResolutionError.DuplicateVariableName(vd);
            }
        }

        private void ResolveSetVariableStatement(QueryDetails details, SetVariableStatement statement)
        {
            if (details.VariableReferences.ContainsKey(statement.VariableReference.Name))
            {
                statement.VariableReference = details.VariableReferences[statement.VariableReference.Name];
            }
            else
            {
                throw NameResolutionError.UnresolvableVariableReference(statement.Variable);
            }
        }

        private void ResolveDeclareTableStatement(QueryDetails details, DeclareTableStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveCreateTableStatement(QueryDetails details, CreateTableStatement statement)
        {
            throw new NotImplementedException();
        }

        // TODO: add alter table here

        private void ResolveDropTableStatement(QueryDetails details, DropTableStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveTruncateTableStatement(QueryDetails details, TruncateTableStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveCreateIndexStatement(QueryDetails details, CreateIndexStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveDropIndexStatement(QueryDetails details, DropIndexStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveSelectStatement(QueryDetails details, SelectStatement statement)
        {
            var cte = statement.FindDescendant<CommonTableExpression>();

            if (cte != null)
            {
                ResolveCommonTableExpression(details, cte);
            }

            ResolveSelect(details, cte, 0, statement);

            var firstqs = statement.QueryExpression.FirstQuerySpecification;

            if (firstqs != null)
            {
                SubstituteOutputTableDefaults(details, firstqs);
                statement.OutputTableReference = ResolveOutputTableReference(details, firstqs);
            }
        }

        private void ResolveInsertStatement(QueryDetails details, InsertStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveUpdateStatement(QueryDetails details, UpdateStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveDeleteStatement(QueryDetails details, DeleteStatement statement)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Expression resolution

        private void ResolveSubtree(QueryDetails details, Node node)
        {
            ResolveSubtree(details, null, null, 0, ColumnContext.None, node);
        }

        private void ResolveSubtree(QueryDetails details, CommonTableExpression cte, QuerySpecification qs, int depth, ColumnContext context, Node node)
        {
            ResolveSubqueries(details, cte, depth, node);
            ResolveExpressionReferences(details, cte, qs, context, node);
        }

        /// <summary>
        /// Resolves an entire subtree of the parsing tree stopping only at 
        /// sustatements and subqueries which
        /// must be resolved recursively prior to calling this function
        /// </summary>
        /// <param name="details"></param>
        /// <param name="cte"></param>
        /// <param name="node"></param>
        /// <param name="context"></param>
        private void ResolveExpressionReferences(QueryDetails details, CommonTableExpression cte, QuerySpecification qs, ColumnContext context, Node node)
        {
            context = GetColumnContext(node, context);

            foreach (var n in node.Nodes)
            {
                if (n is Subquery || n is Statement)
                {
                    return;
                }

                if (n is Node)
                {
                    ResolveExpressionReferences(details, cte, qs, context, (Node)n);   // Recursive call

                    if (n is IFunctionReference)
                    {
                        ResolveFunctionReference((IFunctionReference)n);
                    }
                    else if (n is IVariableReference)
                    {
                        ResolveScalarVariableReference(details, (IVariableReference)n);
                    }
                    else if (n is IColumnReference)
                    {
                        ResolveColumnReference(details, cte, qs, context, (IColumnReference)n);
                    }
                }
            }
        }

        protected virtual bool IsSystemFunctionName(string name)
        {
            return SystemFunctionNames.Contains(name);
        }

        private void ResolveFunctionReference(IFunctionReference node)
        {
            // TODO: extend this to CLR static function calls

            SubstituteFunctionDefaults(node);

            if (!node.FunctionReference.IsUdf)
            {
                if (!IsSystemFunctionName(node.FunctionReference.SystemFunctionName))
                {
                    throw NameResolutionError.UnknownFunctionName(node);
                }
            }
            else
            {
                // Check if dataset specified and make sure it's valid
                if (node.FunctionReference.DatasetName != null)
                {
                    if (!schemaManager.Datasets.ContainsKey(node.FunctionReference.DatasetName))
                    {
                        throw NameResolutionError.UnresolvableDatasetReference(node);
                    }
                }

                var ds = schemaManager.Datasets[node.FunctionReference.DatasetName];

                var dbo = ds.GetObject(node.FunctionReference.DatabaseName, node.FunctionReference.SchemaName, node.FunctionReference.DatabaseObjectName);

                if (dbo == null)
                {
                    throw NameResolutionError.UnresolvableFunctionReference(node);
                }

                node.FunctionReference.DatabaseObject = dbo;
            }
        }

        private void ResolveScalarVariableReference(QueryDetails details, IVariableReference vr)
        {
            // TODO: extend this to UDTs, including member access

            if (vr.VariableReference.Type == VariableReferenceType.System)
            {
                var name = vr.VariableReference.Name.TrimStart('@');

                if (!SystemVariableNames.Contains(name))
                {
                    throw NameResolutionError.UnresolvableVariableReference(vr);
                }
            }
            else if (details.VariableReferences.ContainsKey(vr.VariableReference.Name))
            {
                vr.VariableReference = details.VariableReferences[vr.VariableReference.Name];

                if (vr.VariableReference.Type != VariableReferenceType.Scalar)
                {
                    throw NameResolutionError.ScalarVariableExpected(vr);
                }
            }
            else
            {
                throw NameResolutionError.UnresolvableVariableReference(vr);
            }
        }

        private void ResolveColumnReference(QueryDetails details, CommonTableExpression cte, QuerySpecification qs, ColumnContext context, IColumnReference cr)
        {
            // Star columns cannot be resolved, treat them separately
            if (!cr.ColumnReference.IsResolved && !cr.ColumnReference.IsStar && !cr.ColumnReference.IsComplexExpression)
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
                                    throw NameResolutionError.AmbigousColumnReference(cr);
                                }

                                ncr = ccr;
                                q++;
                            }
                        }
                    }
                }
                else
                {
                    // This has a table reference already so only check
                    // columns of that particular table
                    foreach (var ccr in cr.ColumnReference.TableReference.ColumnReferences)
                    {
                        if (cr.ColumnReference.Compare(ccr))
                        {
                            if (q != 0)
                            {
                                throw NameResolutionError.AmbigousColumnReference(cr);
                            }

                            ncr = ccr;
                            q++;
                        }
                    }
                }

                if (q == 0)
                {
                    throw NameResolutionError.UnresolvableColumnReference(cr);
                }

                // Column context must be updated on source
                ncr.ColumnContext |= context;
                ncr.IsResolved = true;

                // Make copy here to preserve alias!
                ncr = new ColumnReference(ncr);

                if (cr.ColumnReference != null && cr.ColumnReference.ColumnAlias != null)
                {
                    ncr.ColumnAlias = cr.ColumnReference.ColumnAlias;
                }
                
                cr.ColumnReference = ncr;
            }
        }

        #endregion
        #region Query constructs

        /// <summary>
        /// Descend into subtree and resolve subqueries bottom-up
        /// </summary>
        /// <param name="node"></param>
        /// <param name="depth"></param>
        private void ResolveSubqueries(QueryDetails details, CommonTableExpression cte, int depth, Node node)
        {
            foreach (var n in node.Nodes)
            {
                if (n is Node && !(n is Subquery))
                {
                    ResolveSubqueries(details, cte, depth, (Node)n);
                }

                if (n is Subquery)
                {
                    ResolveSelect(details, cte, depth + 1, (Subquery)n);
                }
            }
        }

        protected void ResolveSelect(QueryDetails details, CommonTableExpression cte, int depth, ISelect select)
        {
            var qe = select.QueryExpression;
            ResolveQueryExpression(details, cte, qe, depth);

            var orderBy = select.OrderByClause;

            if (orderBy != null)
            {
                var qs = qe.EnumerateQuerySpecifications().FirstOrDefault();
                ResolveOrderByClause(details, cte, orderBy, qs);
            }
        }

        private void ResolveCommonTableExpression(QueryDetails details, CommonTableExpression cte)
        {
            foreach (var ct in cte.EnumerateCommonTableSpecifications())
            {
                // Because CTEs can reference themselves (i.e. recursive queries) make sure
                // the specification is added to the dictionary before the resolver
                // is called on it
                cte.CommonTableReferences.Add(ct.TableReference.Alias, ct.TableReference);
                ResolveCommonTableSpecification(details, cte, ct);
            }
        }

        private void ResolveCommonTableSpecification(QueryDetails details, CommonTableExpression cte, CommonTableSpecification ts)
        {
            var subquery = ts.Subquery;
            ResolveSelect(details, cte, 1, subquery);
        }

        protected void ResolveQueryExpression(QueryDetails details, CommonTableExpression cte, QueryExpression qe, int depth)
        {
            // Resolve the first part of the query expression independently
            // and make sure it's set as ResultsTableReference
            // This is necessary for CTE evaluation which can be recursive

            int q = 0;

            // Resolve query specifications in the FROM clause
            foreach (var qs in qe.EnumerateDescendants<QuerySpecification>())
            {
                ResolveQuerySpecification(details, cte, qs, depth);

                if (q == 0)
                {
                    // Copy select list columns from the very first query specification.
                    // All subsequent query specifications combined with set operators
                    // (UNION, UNION ALL etc.) must mach the column list.
                    qe.ResultsTableReference.ColumnReferences.AddRange(qs.ResultsTableReference.ColumnReferences);
                }

                q++;
            }
        }

        /// <summary>
        /// Internal routine to perform the name resolution steps on a single
        /// query specification
        /// </summary>
        /// <param name="qs"></param>
        protected void ResolveQuerySpecification(QueryDetails details, CommonTableExpression cte, QuerySpecification qs, int depth)
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
                ResolveSelect(details, cte, depth + 1, sq);
            }

            SubstituteSourceTableDefaults(details, cte, qs);
            CollectSourceTableReferences(details, cte, qs);

            // Column identifiers can contain table names, aliases or nothing,
            // resolve them now
            ResolveTableReferences(details, cte, qs);

            // Substitute SELECT * expressions
            SubstituteStars(qs);

            // Resolve variables and column references of each occurance
            ResolveExpressionReferences(details, cte, qs, ColumnContext.None, qs);

            // Copy resultset columns to the appropriate collection
            CopyResultsColumns(qs);

            // Add default aliases to column expressions in the form of tablealias_columnname
            AssignDefaultColumnAliases(qs, depth != 0);
        }

        protected void ResolveOrderByClause(QueryDetails details, CommonTableExpression cte, OrderByClause orderBy, QuerySpecification firstqs)
        {
            if (orderBy != null)
            {
                ResolveTableReferences(details, cte, firstqs, TableContext.None, ColumnContext.OrderBy, orderBy);
                ResolveExpressionReferences(details, cte, firstqs, ColumnContext.OrderBy, orderBy);
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
        private void CollectSourceTableReferences(QueryDetails details, CommonTableExpression cte, QuerySpecification qs)
        {
            // Collect column references from subqueries or load from the database schema

            foreach (var tr in qs.EnumerateSourceTableReferences(false))
            {
                var exportedName = tr.ExportedName;

                // Make sure that table key is used only once
                if (qs.SourceTableReferences.ContainsKey(exportedName))
                {
                    throw NameResolutionError.DuplicateTableAlias(exportedName, tr.Node);
                }
                else
                {
                    var ntr = ResolveSourceTableReference(details, cte, tr);

                    // Save the table in the query specification
                    qs.SourceTableReferences.Add(exportedName, ntr);
                }
            }
        }

        public TableReference ResolveSourceTableReference(QueryDetails details, CommonTableExpression cte, TableReference tr)
        {
            TableReference ntr;

            if (cte != null && tr.IsPossiblyAlias && cte.CommonTableReferences.ContainsKey(tr.ExportedName))
            {
                // This is a reference to a CTE query

                ntr = new TableReference(cte.CommonTableReferences[tr.DatabaseObjectName]);
                ntr.Type = TableReferenceType.CommonTable;
            }
            else if (tr.Type != TableReferenceType.Subquery && !tr.IsComputed)
            {
                // This is a direct reference to a table or a view but not to a function or subquery

                ntr = tr;

                // Load table description from underlying schema
                // Attempt to load dataset and throw exception of name cannot be resolved
                DatasetBase ds;

                try
                {
                    ds = schemaManager.Datasets[ntr.DatasetName];
                }
                catch (KeyNotFoundException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, ntr);
                }
                catch (SchemaException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, ntr);
                }

                ntr.DatabaseObject = ds.GetObject(ntr.DatabaseName, ntr.SchemaName, ntr.DatabaseObjectName);

                // Load column descriptions for the table
                ntr.LoadColumnReferences(schemaManager);
            }
            else
            {
                // This is a reference to a subquery with an obligatory alias
                ntr = new TableReference(tr);
            }

            ntr.IsResolved = true;

            return ntr;
        }

        private TableReference ResolveOutputTableReference(QueryDetails details, QuerySpecification qs)
        {
            var into = qs.IntoClause;
            var tr = into?.TableName.TableReference;

            if (tr != null)
            {
                DatasetBase ds;

                try
                {
                    ds = schemaManager.Datasets[tr.DatasetName];
                }
                catch (KeyNotFoundException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, tr);
                }
                catch (SchemaException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, tr);
                }

                if (!ds.IsMutable)
                {
                    throw NameResolutionError.TargetDatasetReadOnly((ITableReference)tr.Node);
                }

                tr.DatabaseObject = ds.GetObject(tr.DatabaseName, tr.SchemaName, tr.DatabaseObjectName);

                if (tr == null)
                {
                    tr.DatabaseObject = new Table(ds)
                    {
                        DatabaseName = tr.DatabaseName ?? ds.DatabaseName,
                        SchemaName = tr.SchemaName ?? ds.DefaultSchemaName,
                        TableName = tr.DatabaseObjectName,
                    };
                }

                // TODO: if it is a new table, consider figuring out the columns from the query

                return tr;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Resolves the table references of all nodes below a query specification
        /// not descending into subqueries
        /// </summary>
        /// <param name="qs"></param>
        private void ResolveTableReferences(QueryDetails details, CommonTableExpression cte, QuerySpecification qs)
        {
            ResolveTableReferences(details, cte, qs, TableContext.None, ColumnContext.None, (Node)qs);
        }

        public void ResolveTableReferences(QueryDetails details, CommonTableExpression cte, TableContext tableContext, ColumnContext columnContext, Node n)
        {
            ResolveTableReferences(details, cte, null, tableContext, columnContext, n);
        }

        /// <summary>
        /// Resolves all table references of all nodes below a node,
        /// not descending into subqueries
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="n"></param>
        private void ResolveTableReferences(QueryDetails details, CommonTableExpression cte, QuerySpecification qs, TableContext tableContext, ColumnContext columnContext, Node n)
        {
            tableContext = GetTableContext(n, tableContext);
            columnContext = GetColumnContext(n, columnContext);

            foreach (object o in n.Nodes)
            {
                // Skip the into and clause and subqueries
                if (o is Node && !(o is IntoClause) && !(o is SubqueryTableSource))
                {
                    ResolveTableReferences(details, cte, qs, tableContext, columnContext, (Node)o);   // Recursive call
                }
            }

            if (n is ITableReference)
            {
                ResolveTableReference(details, cte, qs, (ITableReference)n, tableContext, columnContext);
            }
        }

        /// <summary>
        /// Resolves a table reference to a table listed in SourceTableReferences
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="tr"></param>
        private void ResolveTableReference(QueryDetails details, CommonTableExpression cte, QuerySpecification qs, ITableReference node, TableContext tableContext, ColumnContext columnContext)
        {
            // Try to resolve the table alias part of a table reference
            // If and alias or table name is specified, this can be done based on
            // the already collected table sources.
            // If no table or alias is specified and the current node is a column reference,
            // where the column is not a complex expression, resolution might be successful by
            // column name only.

            // TODO: add support for variables

            if (node.TableReference != null && !node.TableReference.IsUndefined && !node.TableReference.IsResolved)
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
                            throw NameResolutionError.UnresolvableDatasetReference(node);
                        }
                    }

                    // if only a table name found and that's not an alias -> must be a table
                    int q = 0;
                    foreach (var tr in qs.SourceTableReferences.Values)
                    {
                        if (tr.Compare(node.TableReference))
                        {
                            if (q != 0)
                            {
                                throw NameResolutionError.AmbigousTableReference(node);
                            }

                            ntr = tr;
                            q++;
                        }
                    }
                }

                if (ntr == null)
                {
                    throw NameResolutionError.UnresolvableTableReference(node);
                }

                ntr.IsResolved = true;

                node.TableReference = ntr;
            }

            // If we are inside a table hint, make sure the reference is to the current table
            if (columnContext == ColumnContext.Hint)
            {
                // In this case a column reference appears inside a table hint (WITH clause)
                // If the table reference is undefined it must refer to the table itself
                // otherwise we must make sure it is indeed referencing the table itself

                var ts = ((Node)node).FindAscendant<SimpleTableSource>();

                if (node.TableReference.IsUndefined)
                {
                    node.TableReference = ts.TableReference;
                }
                else if (node.TableReference != ts.TableReference)
                {
                    throw CreateException(ExceptionMessages.DifferentTableReferenceInHintNotAllowed, (Node)node);
                }
            }
        }

        protected virtual TableContext GetTableContext(Node n, TableContext context)
        {
            if (n is FromClause)
            {
                context = TableContext.From;
            }
            else if (n is IntoClause)
            {
                context = TableContext.Into;
            }
            else if (n is Subquery)
            {
                context = TableContext.Subquery;
            }

            return context;
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
            else if (n is TableHintClause)
            {
                context = ColumnContext.Hint;
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
                        n.Value = SubstituteStars((SelectList)n.Value);
                    }

                    n = n.Next;
                }
            }
        }

        #region Default substitution logic

        /// <summary>
        /// Substitutes dataset and schema defaults into table source table references
        /// </summary>
        /// <param name="qs"></param>
        protected void SubstituteSourceTableDefaults(QueryDetails details, CommonTableExpression cte, QuerySpecification qs)
        {
            foreach (var tr in qs.EnumerateSourceTableReferences(false))
            {
                try
                {
                    if (cte != null && tr.IsPossiblyAlias && cte.CommonTableReferences.ContainsKey(tr.DatabaseObjectName))
                    {
                        // Don't do any substitution if referencing a common table
                    }
                    else if (tr.Type == TableReferenceType.TableOrView)
                    {
                        tr.SubstituteDefaults(SchemaManager, defaultTableDatasetName);
                    }
                    else if (tr.Type == TableReferenceType.UserDefinedFunction)
                    {
                        tr.SubstituteDefaults(SchemaManager, defaultFunctionDatasetName);
                    }
                }
                catch (KeyNotFoundException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, tr);
                }
            }
        }

        private void SubstituteOutputTableDefaults(QueryDetails details, QuerySpecification qs)
        {
            // TODO: what to do with table variables?

            var tr = qs.IntoClause?.TableName?.TableReference;

            if (tr != null)
            {
                tr.Type = TableReferenceType.SelectInto;
                tr.SubstituteDefaults(schemaManager, defaultOutputDatasetName);
            }
        }

        /// <summary>
        /// Substitutes dataset and schema defaults into function references.
        /// </summary>
        /// <remarks>
        /// This is non-standard SQL as SQL requires the schema name to be specified and the database is
        /// always taken from the current context. In applications, like SkyQuery, functions are always
        /// taken from the CODE database.
        /// </remarks>
        /// <param name="node"></param>
        private void SubstituteFunctionDefaults(IFunctionReference node)
        {
            if (!node.FunctionReference.IsSystem)
            {
                try
                {
                    node.FunctionReference.SubstituteDefaults(SchemaManager, defaultFunctionDatasetName);
                }
                catch (KeyNotFoundException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, node);
                }
            }
        }

        public SelectList SubstituteStars(SelectList selectList)
        {
            var ce = selectList.FindDescendant<ColumnExpression>();
            var subsl = selectList.FindDescendant<SelectList>();

            SelectList sl = null;
            QuerySpecification qs = null;

            if (ce.ColumnReference.IsStar)
            {
                // Build select list from the column list of
                // the referenced table, then replace current node
                if (ce.TableReference.IsUndefined)
                {
                    qs = selectList.FindAscendant<QuerySpecification>();
                    sl = SelectList.Create(qs);
                }
                else
                {
                    sl = SelectList.Create(ce.TableReference);
                }

                if (subsl != null)
                {
                    sl.Append(SubstituteStars(subsl));
                }

                return sl;
            }
            else
            {
                if (subsl != null)
                {
                    selectList.Replace(SubstituteStars(subsl));
                }

                return selectList;
            }
        }

        /// <summary>
        /// Adds default aliases to columns with no aliases specified in the query
        /// </summary>
        /// <param name="qs"></param>
        private void AssignDefaultColumnAliases(QuerySpecification qs, bool subquery)
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
                        if (subquery)
                        {
                            throw NameResolutionError.MissingColumnAlias(ce);
                        }
                        else
                        {
                            alias = GetUniqueColumnAlias(aliases, String.Format("Col_{0}", cr.SelectListIndex));
                        }
                    }
                    else if (!subquery)
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
                    else
                    {
                        alias = null;
                    }
                }
                else
                {
                    // Alias is set explicitly, so do not make it unique forcibly
                    alias = cr.ColumnAlias;
                }

                if (alias != null)
                {
                    if (aliases.Contains(alias))
                    {
                        throw NameResolutionError.DuplicateColumnAlias(alias, ce);
                    }

                    aliases.Add(alias);
                    cr.ColumnAlias = alias;
                }
            }
        }

        #endregion

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

        #endregion

        private void ResolveDataTypeReferences()
        {
            throw new NotImplementedException();
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
