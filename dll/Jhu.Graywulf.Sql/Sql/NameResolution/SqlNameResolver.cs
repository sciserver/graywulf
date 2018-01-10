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

        // The schema manager is used to resolve identifiers that are not local to the script,
        // i.e. database, table, columns etc. names
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
        public void Execute(StatementBlock script)
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

            // TODO: move this somewhere inside
            SubstituteFunctionDefaults((Node)script);
            ResolveFunctionReferences((Node)script);

            ResolveStatementBlock(script);
        }

        protected void ResolveStatementBlock(StatementBlock script)
        {
            foreach (var statement in script.EnumerateDescendants<Statement>(true))
            {
                ResolveStatement(script, statement);
            }
        }

        private void ResolveStatement(StatementBlock script, Statement statement)
        {
            var s = statement.SpecificStatement;

            // Call recursively for sub-statements
            foreach (var ss in s.EnumerateSubStatements())
            {
                ResolveStatement(script, ss);
            }

            if (s.IsResolvable)
            {
                // Resolve current statement
                switch (s)
                {
                    case WhileStatement ss:
                        ResolveWhileStatement(script, ss);
                        break;
                    case ReturnStatement ss:
                        ResolveReturnStatement(script, ss);
                        break;
                    case IfStatement ss:
                        ResolveIfStatement(script, ss);
                        break;
                    case ThrowStatement ss:
                        ResolveThrowStatement(script, ss);
                        break;
                    case DeclareCursorStatement ss:
                        ResolveDeclareCursorStatement(script, ss);
                        break;
                    case SetCursorStatement ss:
                        ResolveSetCursorStatement(script, ss);
                        break;
                    case CursorOperationStatement ss:
                        ResolveCursorOperationStatement(script, ss);
                        break;
                    case FetchStatement ss:
                        ResolveFetchStatement(script, ss);
                        break;
                    case DeclareVariableStatement ss:
                        ResolveDeclareVariableStatement(script, ss);
                        break;
                    case SetVariableStatement ss:
                        ResolveSetVariableStatement(script, ss);
                        break;
                    case DeclareTableStatement ss:
                        ResolveDeclareTableStatement(script, ss);
                        break;
                    case CreateTableStatement ss:
                        ResolveCreateTableStatement(script, ss);
                        break;
                    case DropTableStatement ss:
                        ResolveDropTableStatement(script, ss);
                        break;
                    case TruncateTableStatement ss:
                        ResolveTruncateTableStatement(script, ss);
                        break;
                    case CreateIndexStatement ss:
                        ResolveCreateIndexStatement(script, ss);
                        break;
                    case DropIndexStatement ss:
                        ResolveDropIndexStatement(script, ss);
                        break;
                    case SelectStatement ss:
                        ResolveSelectStatement(script, ss);
                        break;
                    case InsertStatement ss:
                        ResolveInsertStatement(script, ss);
                        break;
                    case DeleteStatement ss:
                        ResolveDeleteStatement(script, ss);
                        break;
                    default:
                        throw new NotImplementedException();
                }

            }
        }

        #region Statement resolution dispatch functions

        private void ResolveWhileStatement(StatementBlock script, WhileStatement statement)
        {
            ResolveBooleanExpression(statement.Condition);
        }

        private void ResolveReturnStatement(StatementBlock script, ReturnStatement statement)
        {
            // it might have a query in the parameter
            throw new NotImplementedException();
        }

        private void ResolveIfStatement(StatementBlock script, IfStatement statement)
        {
            ResolveBooleanExpression(statement.Condition);
        }

        private void ResolveThrowStatement(StatementBlock script, ThrowStatement statement)
        {
            // Resolve variables
            throw new NotImplementedException();
        }

        private void ResolveDeclareCursorStatement(StatementBlock script, DeclareCursorStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveSetCursorStatement(StatementBlock script, SetCursorStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveCursorOperationStatement(StatementBlock script, CursorOperationStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveFetchStatement(StatementBlock script, FetchStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveDeclareVariableStatement(StatementBlock script, DeclareVariableStatement statement)
        {
            foreach (var vd in statement.EnumerateDescendantsRecursive<VariableDeclaration>())
            {
                ResolveVariableDeclaration(script, vd);
            }
        }

        private void ResolveVariableDeclaration(StatementBlock script, VariableDeclaration vd)
        {
            if (!script.VariableReferences.ContainsKey(vd.VariableReference.Name))
            {
                script.VariableReferences.Add(vd.VariableReference.Name, vd.VariableReference);
            }
            else
            {
                throw NameResolutionError.DuplicateVariableName(vd);
            }
        }

        private void ResolveSetVariableStatement(StatementBlock script, SetVariableStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveDeclareTableStatement(StatementBlock script, DeclareTableStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveCreateTableStatement(StatementBlock script, CreateTableStatement statement)
        {
            throw new NotImplementedException();
        }

        // TODO: add alter table here

        private void ResolveDropTableStatement(StatementBlock script, DropTableStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveTruncateTableStatement(StatementBlock script, TruncateTableStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveCreateIndexStatement(StatementBlock script, CreateIndexStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveDropIndexStatement(StatementBlock script, DropIndexStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveSelectStatement(StatementBlock script, SelectStatement statement)
        {
            // CTE
            var cte = statement.FindDescendant<CommonTableExpression>();
            if (cte != null)
            {
                // TODO: ResolveCommonTableExpression();
            }

            ResolveSelectStatement(script, statement, 0);
        }

        private void ResolveInsertStatement(StatementBlock script, InsertStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveUpdateStatement(StatementBlock script, UpdateStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveDeleteStatement(StatementBlock script, DeleteStatement statement)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Boolean expression resolution

        private void ResolveBooleanExpression(BooleanExpression condition)
        {
            throw new NotImplementedException();
        }

        #endregion


        // TODO: make this protected once full script support is implemented
        public void ResolveSelectStatement(StatementBlock script, SelectStatement select, int depth)
        {
            var qe = select.QueryExpression;

            ResolveQueryExpression(script, qe, depth);

            var firstqs = qe.FindDescendant<QuerySpecification>();
            var orderby = select.OrderByClause;

            ResolveOrderByClause(script, orderby, firstqs);
        }

        protected void ResolveSubquery(StatementBlock script, Subquery subquery, int depth)
        {
            var qe = subquery.FindDescendant<QueryExpression>();
            ResolveQueryExpression(script, qe, depth);

            var qs = qe.EnumerateQuerySpecifications().FirstOrDefault();
            var orderBy = subquery.FindDescendant<OrderByClause>();

            if (orderBy != null)
            {
                ResolveOrderByClause(script, orderBy, qs);
            }
        }

        protected void ResolveQueryExpression(StatementBlock script, QueryExpression qe, int depth)
        {
            // Resolve query specifications in the FROM clause
            foreach (var qs in qe.EnumerateDescendants<QuerySpecification>())
            {
                ResolveQuerySpecification(script, qs, depth);
            }

            // Copy select list columns from the very first query specification.
            // All subsequent query specifications combined with set operators
            // (UNION, UNION ALL etc.) must mach the column list.
            var firstqs = qe.FindDescendant<QuerySpecification>();
            qe.ResultsTableReference.ColumnReferences.AddRange(firstqs.ResultsTableReference.ColumnReferences);
        }

        /// <summary>
        /// Internal routine to perform the name resolution steps on a single
        /// query specification
        /// </summary>
        /// <param name="qs"></param>
        protected void ResolveQuerySpecification(StatementBlock script, QuerySpecification qs, int depth)
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
                ResolveSubquery(script, sq, depth + 1);
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

            // Resolve variables and column references of each occurance
            ResolveVariables(script, qs);

            // Copy resultset columns to the appropriate collection
            CopyResultsColumns(qs);

            // Add default aliases to column expressions in the form of tablealias_columnname
            AssignDefaultColumnAliases(qs, depth != 0);
        }

        protected void ResolveOrderByClause(StatementBlock script, OrderByClause orderBy, QuerySpecification firstqs)
        {
            if (orderBy != null)
            {
                ResolveTableReferences(firstqs, orderBy, ColumnContext.OrderBy);
                ResolveVariables(script, firstqs, orderBy, ColumnContext.OrderBy);
            }
        }

        // ----------------------------------

        private void ResolveDataTypeReferences()
        {
            throw new NotImplementedException();
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
                if (tr.Type == TableReferenceType.Subquery || tr.Type == TableReferenceType.UserDefinedFunction || tr.IsComputed || tr.Alias != null)
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
                    throw NameResolutionError.DuplicateTableAlias(tablekey, tr.Node);
                }
                else
                {
                    var ntr = ResolveSourceTableReference(tr);

                    qs.SourceTableReferences.Add(tablekey, ntr);
                }
            }
        }

        public TableReference ResolveSourceTableReference(TableReference tr)
        {
            var ntr = new TableReference(tr);

            if (ntr.Type != TableReferenceType.Subquery && !ntr.IsComputed)
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

            return ntr;
        }

        /// <summary>
        /// Resolves the table references of all nodes below a query specification
        /// not descending into subqueries
        /// </summary>
        /// <param name="qs"></param>
        private void ResolveTableReferences(QuerySpecification qs)
        {
            ResolveTableReferences(qs, (Node)qs, ColumnContext.None);
        }

        public void ResolveTableReferences(Node n, ColumnContext context)
        {
            ResolveTableReferences(null, n, context);
        }

        /// <summary>
        /// Resolves all table references of all nodes below a node,
        /// not descending into subqueries
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="n"></param>
        private void ResolveTableReferences(QuerySpecification qs, Node n, ColumnContext context)
        {
            context = GetColumnContext(n, context);

            foreach (object o in n.Nodes)
            {
                // Skip the into and clause and subqueries
                if (!(o is IntoClause) && !(o is SubqueryTableSource))
                {
                    if (o is Node)
                    {
                        ResolveTableReferences(qs, (Node)o, context);   // Recursive call
                    }
                }
            }

            if (n is ITableReference && ((ITableReference)n).TableReference != null)
            {
                ResolveTableReference(qs, (ITableReference)n, context);
            }
        }

        /// <summary>
        /// Resolves a table reference to a table listed in SourceTableReferences
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="tr"></param>
        private void ResolveTableReference(QuerySpecification qs, ITableReference node, ColumnContext context)
        {
            // Try to resolve the table alias part of a table reference
            // If and alias or table name is specified, this can be done based on
            // the already collected table sources.
            // If no table or alias is specified and the current node is a column reference,
            // where the column is not a complex expression, resolution might be successful by
            // column name only.

            // TODO: add support for variables

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

                node.TableReference = ntr;
            }

            // If we are inside a table hint, make sure the reference is to the current table
            if (context == ColumnContext.Hint)
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

        private void ResolveVariables(StatementBlock script, QuerySpecification qs)
        {
            ResolveVariables(script, qs, (Node)qs, ColumnContext.None);
        }

        /// <summary>
        /// Resolves all table references of all nodes below a node,
        /// not descending into subqueries
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="n"></param>
        private void ResolveVariables(StatementBlock script, QuerySpecification qs, Node n, ColumnContext context)
        {
            context = GetColumnContext(n, context);

            foreach (object o in n.Nodes)
            {
                // Skip the into clause and subqueries
                // Subqueries are already processed recursively.
                if (!(o is IntoClause) && !(o is SubqueryTableSource))
                {
                    if (o is Node)
                    {
                        ResolveVariables(script, qs, (Node)o, context);   // Recursive call
                    }
                }
            }

            // TODO: extend this to CLR static function calls

            if (n is IVariableReference)
            {
                ResolveScalarVariableReference(script, (IVariableReference)n);
            }
            else if (n is IColumnReference)
            {
                ResolveColumnReference(qs, (IColumnReference)n, context);
            }
        }

        private void ResolveScalarVariableReference(StatementBlock script, IVariableReference vr)
        {
            // TODO: extend this to UDTs

            if (vr.VariableReference.IsCursor || vr.VariableReference.IsTable)
            {
                throw NameResolutionError.ScalarVariableExpected(vr);
            }

            if (script.VariableReferences.ContainsKey(vr.VariableReference.Name))
            {
                vr.VariableReference = script.VariableReferences[vr.VariableReference.Name];
            }
            else
            {
                throw NameResolutionError.UnresolvableVariableReference(vr);
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
                                    throw NameResolutionError.AmbigousColumnReference(cr);
                                }

                                ncr = ccr;
                                q++;
                            }
                        }
                    }
                }
                else if (!cr.ColumnReference.TableReference.IsUndefined)
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
            // *** TODO: handle sys functions here
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

        protected virtual bool IsSystemFunctionName(string name)
        {
            return SystemFunctionNames.Contains(name);
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

        #region Default substitution logic

        /// <summary>
        /// Substitutes dataset and schema defaults into table source table references
        /// </summary>
        /// <param name="qs"></param>
        protected void SubstituteTableAndColumnDefaults(QuerySpecification qs)
        {
            foreach (var tr in qs.EnumerateSourceTableReferences(false))
            {
                try
                {
                    if (tr.Type == TableReferenceType.TableOrView)
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

        /// <summary>
        /// Substitutes dataset and schema defaults into function references.
        /// </summary>
        /// <remarks>
        /// This is non-standard SQL as SQL requires the schema name to be specified and the database is
        /// always taken from the current context. In applications, like SkyQuery, functions are always
        /// taken from the CODE database.
        /// </remarks>
        /// <param name="node"></param>
        protected void SubstituteFunctionDefaults(Node node)
        {
            foreach (var fi in node.EnumerateDescendantsRecursive<FunctionIdentifier>())
            {
                try
                {
                    fi.FunctionReference.SubstituteDefaults(SchemaManager, defaultFunctionDatasetName);
                }
                catch (KeyNotFoundException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, fi);
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
