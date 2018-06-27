using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class SqlNameResolver
    {
        #region Constants 
        private static readonly HashSet<string> SystemFunctionNames = new HashSet<string>(Schema.SchemaManager.Comparer)
        {
            "COALESCE", "NULLIF",

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
            "TEXTSIZE", "VERSION",

            // Custom system variables
            Constants.SystemVariableNamePartCount,
            Constants.SystemVariableNamePartId,
        };

        #endregion
        #region Private member variables

        // The schema manager is used to resolve identifiers that are not local to the details,
        // i.e. database, table, columns etc. names
        private SchemaManager schemaManager;
        private QueryDetails details;

        private string defaultTableDatasetName;
        private string defaultFunctionDatasetName;
        private string defaultDataTypeDatasetName;
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

        public string DefaultDataTypeDatasetName
        {
            get { return defaultDataTypeDatasetName; }
            set { defaultDataTypeDatasetName = value; }
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
            this.defaultDataTypeDatasetName = String.Empty;
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
            this.details = details;
            ResolveStatementBlock(details.ParsingTree);
            details.IsResolved = true;
        }

        protected void ResolveStatementBlock(StatementBlock statementBlock)
        {
            foreach (var statement in statementBlock.EnumerateDescendants<Statement>(true))
            {
                ResolveStatement(statement);
            }
        }

        private void ResolveStatement(Statement statement)
        {
            var s = statement.SpecificStatement;

            // Resolve current statement
            if (s.IsResolvable)
            {
                switch (s)
                {
                    case WhileStatement ss:
                        ResolveWhileStatement(ss);
                        break;
                    case ReturnStatement ss:
                        ResolveReturnStatement(ss);
                        break;
                    case IfStatement ss:
                        ResolveIfStatement(ss);
                        break;
                    case ThrowStatement ss:
                        ResolveThrowStatement(ss);
                        break;
                    case DeclareVariableStatement ss:
                        ResolveDeclareVariableStatement(ss);
                        break;
                    case DeclareTableStatement ss:
                        ResolveDeclareTableStatement(ss);
                        break;
                    case DeclareCursorStatement ss:
                        ResolveDeclareCursorStatement(ss);
                        break;
                    case SetCursorStatement ss:
                        ResolveSetCursorStatement(ss);
                        break;
                    case CursorOperationStatement ss:
                        ResolveCursorOperationStatement(ss);
                        break;
                    case FetchStatement ss:
                        ResolveFetchStatement(ss);
                        break;
                    case SetVariableStatement ss:
                        ResolveSetVariableStatement(ss);
                        break;
                    case CreateTableStatement ss:
                        ResolveCreateTableStatement(ss);
                        break;
                    case DropTableStatement ss:
                        ResolveDropTableStatement(ss);
                        break;
                    case TruncateTableStatement ss:
                        ResolveTruncateTableStatement(ss);
                        break;
                    case CreateIndexStatement ss:
                        ResolveCreateIndexStatement(ss);
                        break;
                    case DropIndexStatement ss:
                        ResolveDropIndexStatement(ss);
                        break;
                    case SelectStatement ss:
                        ResolveSelectStatement(ss);
                        break;
                    case InsertStatement ss:
                        ResolveInsertStatement(ss);
                        break;
                    case DeleteStatement ss:
                        ResolveDeleteStatement(ss);
                        break;
                    case UpdateStatement us:
                        ResolveUpdateStatement(us);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            // Call recursively for sub-statements
            foreach (var ss in s.EnumerateSubStatements())
            {
                ResolveStatement(ss);
            }
        }

        private void ResolveWhileStatement(WhileStatement statement)
        {
            ResolveSubtree(QueryContext.None, statement.Condition);
        }

        private void ResolveReturnStatement(ReturnStatement statement)
        {
            // it might have a query in the parameter
            // do we support functions or stored procedures?
            throw new NotImplementedException();
        }

        private void ResolveIfStatement(IfStatement statement)
        {
            ResolveSubtree(QueryContext.None, statement.Condition);
        }

        private void ResolveThrowStatement(ThrowStatement statement)
        {
            // Resolve variables
            throw new NotImplementedException();
        }

        private void ResolveDeclareCursorStatement(DeclareCursorStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveSetCursorStatement(SetCursorStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveCursorOperationStatement(CursorOperationStatement statement)
        {
            throw new NotImplementedException();
        }

        private void ResolveFetchStatement(FetchStatement statement)
        {
            throw new NotImplementedException();
        }

        #region Scalar and table-valued variables

        private void ResolveDeclareVariableStatement(DeclareVariableStatement statement)
        {
            foreach (var vd in statement.EnumerateDescendantsRecursive<VariableDeclaration>())
            {
                ResolveVariableDeclaration(vd);
            }
        }

        private void ResolveVariableDeclaration(VariableDeclaration vd)
        {
            SubstituteDataTypeDefaults(vd.DataType.DataTypeReference);
            ResolveDataTypeReference(vd.DataType.DataTypeReference);

            // Resolve value assignment expression
            var exp = vd.Expression;
            if (exp != null)
            {
                ResolveSubtree(QueryContext.None, exp);
            }

            var vr = vd.VariableReference;
            var variable = CreateVariable(vr);

            // Add to query details
            if (!details.VariableReferences.ContainsKey(vd.VariableReference.VariableName))
            {
                details.VariableReferences.Add(vd.VariableReference.VariableName, vd.VariableReference);
            }
            else
            {
                throw NameResolutionError.DuplicateVariableName(vd);
            }
        }

        private Schema.Variable CreateVariable(VariableReference vr)
        {
            var variable = new Schema.Variable()
            {
                Name = vr.VariableName,
                DataType = vr.DataTypeReference.DataType,
            };

            return variable;
        }

        private void ResolveSetVariableStatement(SetVariableStatement statement)
        {
            if (details.VariableReferences.ContainsKey(statement.VariableReference.VariableName))
            {
                statement.VariableReference = details.VariableReferences[statement.VariableReference.VariableName];
            }
            else
            {
                throw NameResolutionError.UnresolvableVariableReference(statement.Variable);
            }
        }

        private void ResolveDeclareTableStatement(DeclareTableStatement statement)
        {
            var vr = statement.VariableReference;
            var td = statement.TableDefinition;
            var dr = new DataTypeReference();
            var tr = new TableReference()
            {
                VariableName = vr.VariableName
            };

            ResolveTableDefinition(td, tr, dr);

            statement.VariableReference.TableReference = tr;
            statement.VariableReference.DataTypeReference = dr;
            
            var variable = CreateVariable(vr);
            var dataType = CreateDataType(dr);

            vr.Variable = variable;
            dr.DataType = dataType;

            // Add to query details
            if (!details.VariableReferences.ContainsKey(vr.VariableName))
            {
                details.VariableReferences.Add(vr.VariableName, vr);
            }
            else
            {
                throw NameResolutionError.DuplicateVariableName(statement.TargetVariable);
            }
        }

        private DataType CreateDataType(DataTypeReference dr)
        {
            var dataType = new Schema.DataType()
            {
                IsTableType = dr.ColumnReferences.Count > 0,
                IsUserDefined = true,
            };


            foreach (var cr in dr.ColumnReferences)
            {
                var col = CreateColumn(cr);
                dataType.Columns.TryAdd(col.ColumnName, col);
            }

            // TODO:
            // indexes
            // primary key
            // metadata

            return dataType;
        }

        #endregion
        #region Table DDL

        private void ResolveCreateTableStatement(CreateTableStatement statement)
        {
            var tr = statement.TargetTable.TableReference;
            var td = statement.TableDefinition;

            SubstituteOutputTableDefaults(null, statement.TargetTable.TableReference, TableContext.CreateTable);
            ResolveTableDefinition(td, tr, null);

            var table = CreateTable(tr);
            if (!table.Dataset.Tables.TryAdd(table.UniqueKey, table))
            {
                throw NameResolutionError.TableAlreadyExists(statement.TargetTable);
            }

            tr.DatabaseObject = table;

            // Add to query details
            if (!details.TargetTableReferences.ContainsKey(table.UniqueKey))
            {
                details.TargetTableReferences.Add(table.UniqueKey, new List<TableReference>());
            }
            details.TargetTableReferences[table.UniqueKey].Add(tr);
        }

        private void ResolveTableDefinition(TableDefinitionList td, TableReference tr, DataTypeReference dr)
        {
            foreach (var item in td.EnumerateTableDefinitionItems())
            {
                var cd = item.ColumnDefinition;
                var tc = item.TableConstraint;
                var ti = item.TableIndex;

                if (cd != null)
                {
                    ResolveColumnDefinition(cd, tr, dr);
                }

                if (tc != null)
                {
                    ResolveTableConstraint(tc);
                }

                if (ti != null)
                {
                    ResolveTableIndex(ti);
                }
            }
        }

        private void ResolveColumnDefinition(ColumnDefinition cd, TableReference tr, DataTypeReference dr)
        {
            SubstituteDataTypeDefaults(cd.DataTypeIdentifier.DataTypeReference);
            ResolveDataTypeReference(cd.DataTypeIdentifier.DataTypeReference);

            cd.DataTypeIdentifier.DataTypeReference.DataType.IsNullable = cd.IsNullable;

            // Column defults contain an expression
            var dd = cd.DefaultDefinition;

            if (dd != null)
            {
                var exp = dd.Expression;
                ResolveSubtree(QueryContext.None, exp);
            }

            var ncr = new ColumnReference(tr, dr, cd.ColumnReference, cd.DataTypeIdentifier.DataTypeReference);

            if (tr != null)
            {
                tr.ColumnReferences.Add(ncr);
            }

            if (dr != null)
            {
                dr.ColumnReferences.Add(ncr);
            }
        }

        private void ResolveTableConstraint(TableConstraint tc)
        {
        }

        private void ResolveTableIndex(TableIndex ti)
        {
        }

        private Table CreateTable(TableReference tr)
        {
            var ds = SchemaManager.Datasets[tr.DatasetName];

            var table = new Schema.Table(ds)
            {
                DatabaseName = tr.DatabaseName,
                SchemaName = tr.SchemaName,
                TableName = tr.DatabaseObjectName
            };

            foreach (var cr in tr.ColumnReferences)
            {
                var col = CreateColumn(cr);
                table.Columns.TryAdd(col.ColumnName, col);
            }

            // TODO:
            // indexes
            // primary key
            // metadata

            return table;
        }

        private Column CreateColumn(ColumnReference cr)
        {
            var column = new Column(cr.ColumnName, cr.DataTypeReference.DataType);
            return column;
        }

        private Table AlterTable(TableReference tr)
        {
            throw new NotImplementedException();
        }

        // TODO: add alter table here

        private void ResolveDropTableStatement(DropTableStatement statement)
        {
            ResolveTargetTable(null, null, statement.TargetTable);

            var table = (Schema.Table)statement.TargetTable.TableReference.DatabaseObject;

            if (!table.Dataset.Tables.TryRemove(table.UniqueKey, out var t))
            {
                throw NameResolutionError.TableDoesNotExists(statement.TargetTable);
            }
        }

        private void ResolveTruncateTableStatement(TruncateTableStatement statement)
        {
            ResolveTargetTable(null, null, statement.TargetTable);
        }

        private void ResolveCreateIndexStatement(CreateIndexStatement statement)
        {
            ResolveTargetTable(null, null, statement.TargetTable);

            // TODO: update index in schema
        }

        private void ResolveDropIndexStatement(DropIndexStatement statement)
        {
            ResolveTargetTable(null, null, statement.TargetTable);

            // TODO: update index in schema
        }

        #endregion

        private void ResolveSelectStatement(SelectStatement statement)
        {
            var cte = statement.FindDescendant<CommonTableExpression>();
            if (cte != null)
            {
                ResolveCommonTableExpression(cte);
            }

            ResolveSelect(cte, 0, QueryContext.SelectStatement, statement);

            var firstqs = statement.QueryExpression.FirstQuerySpecification;
            if (firstqs != null)
            {
                var into = firstqs.IntoClause?.TargetTable;
                if (into != null)
                {
                    ResolveOutputTable(cte, into);
                }
            }
        }

        private void ResolveInsertStatement(InsertStatement statement)
        {
            // Resolve CTE
            var cte = statement.FindDescendant<CommonTableExpression>();
            if (cte != null)
            {
                ResolveCommonTableExpression(cte);
            }

            // Resolve target table
            var target = statement.TargetTable;
            ResolveTargetTable(cte, statement, target);

            // Resolve column list
            var columnList = statement.ColumnList;
            if (columnList != null)
            {
                foreach (var column in columnList.EnumerateDescendants<ColumnIdentifier>())
                {
                    // Resolve column name and make sure it belongs to the target table
                    ResolveColumnReference(null, new[] { target.TableReference }, ColumnContext.Insert, column);

                    // Make sure column belongs to the target table
                    if (!column.ColumnReference.ParentTableReference.Compare(target.TableReference))
                    {
                        throw NameResolutionError.ColumnNotPartOfTargetTable(column);
                    }
                }
            }

            // Resolve values list (incl. subqueries)
            var values = statement.ValuesClause;
            if (values != null)
            {
                foreach (var group in values.EnumerateValueGroups())
                {
                    foreach (var val in group.EnumerateValues())
                    {
                        // It can be an Expression or DEFAULT, we ignore the latter
                        if (val is Expression exp)
                        {
                            ResolveSubqueries(cte, 0, QueryContext.InsertStatement, exp);
                            ResolveExpressionReferences(cte, statement, ColumnContext.None, exp);
                        }
                    }
                }
            }

            // Resolve select part
            var query = statement.QueryExpression;
            if (query != null)
            {
                ResolveQueryExpression(cte, query, 0, QueryContext.InsertStatement | QueryContext.SelectStatement);

                // Resolve order by
                var orderBy = statement.OrderByClause;
                if (orderBy != null)
                {
                    var qs = query.EnumerateQuerySpecifications().FirstOrDefault();
                    ResolveOrderByClause(cte, orderBy, qs, QueryContext.InsertStatement | QueryContext.SelectStatement);
                }
            }
        }

        private void ResolveDeleteStatement(DeleteStatement statement)
        {
            // Resolve CTE
            var cte = statement.FindDescendant<CommonTableExpression>();
            if (cte != null)
            {
                ResolveCommonTableExpression(cte);
            }

            // Resolve target table
            var target = statement.TargetTable;
            ResolveTargetTable(cte, statement, target);

            var from = statement.FromClause;
            if (from != null)
            {
                ResolveTableAndColumnReferences(cte, statement, from, from, 0, QueryContext.DeleteStatement);
            }

            var where = statement.WhereClause;
            if (where != null)
            {
                ResolveTableAndColumnReferences(cte, statement, where, where, 0, QueryContext.DeleteStatement);
            }
        }

        private void ResolveUpdateStatement(UpdateStatement statement)
        {
            // Resolve CTE
            var cte = statement.FindDescendant<CommonTableExpression>();
            if (cte != null)
            {
                ResolveCommonTableExpression(cte);
            }

            var from = statement.FromClause;
            if (from != null)
            {
                ResolveTableAndColumnReferences(cte, statement, from, from, 0, QueryContext.DeleteStatement);
            }

            var where = statement.WhereClause;
            if (where != null)
            {
                ResolveTableAndColumnReferences(cte, statement, where, where, 0, QueryContext.DeleteStatement);
            }

            // Resolve target table
            var target = statement.TargetTable;
            ResolveTargetTable(cte, statement, target);

            // Resolve values list (incl. subqueries)
            var sets = statement.UpdateSetList;
            if (sets != null)
            {
                foreach (var set in sets.EnumerateSetColumns())
                {
                    var leftvar = set.LeftHandSide.UserVariable;
                    if (leftvar != null)
                    {
                        ResolveScalarVariableReference(leftvar);
                    }

                    var leftcol = set.LeftHandSide.ColumnIdentifier;
                    if (leftcol != null)
                    {
                        ResolveColumnReference(cte, new[] { target.TableReference }, ColumnContext.Update, leftcol);
                    }

                    var rightexp = set.RightHandSide.Expression;
                    if (rightexp != null)
                    {
                        ResolveSubqueries(cte, 0, QueryContext.InsertStatement, rightexp);
                        ResolveExpressionReferences(cte, statement, ColumnContext.None, rightexp);
                    }
                }
            }
        }

        #endregion
        #region Expression resolution

        private void ResolveSubtree(QueryContext queryContext, Node node)
        {
            ResolveSubtree(null, null, 0, queryContext, ColumnContext.None, node);
        }

        private void ResolveSubtree(CommonTableExpression cte, QuerySpecification qs, int depth, QueryContext queryContext, ColumnContext columnContext, Node node)
        {
            ResolveSubqueries(cte, depth, queryContext, node);
            ResolveExpressionReferences(cte, qs, columnContext, node);
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
        private void ResolveExpressionReferences(CommonTableExpression cte, ISourceTableCollection resolvedSourceTables, ColumnContext context, Node node)
        {
            context = GetColumnContext(node, context);

            foreach (var n in node.Nodes)
            {
                if (n is Subquery ||
                    n is VariableTableSource ||
                    n is Statement)
                {
                    return;
                }

                if (n is Node)
                {
                    ResolveExpressionReferences(cte, resolvedSourceTables, context, (Node)n);   // Recursive call

                    if (n is IFunctionReference)
                    {
                        ResolveFunctionReference((IFunctionReference)n);
                    }
                    else if (n is IVariableReference)
                    {
                        ResolveScalarVariableReference((IVariableReference)n);
                    }
                    else if (n is IColumnReference)
                    {
                        ResolveColumnReference(cte, resolvedSourceTables.ResolvedSourceTableReferences.Values, context, (IColumnReference)n);
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

                var uniqueKey = node.FunctionReference.DatabaseObject.UniqueKey;

                if (!details.FunctionReferences.ContainsKey(uniqueKey))
                {
                    details.FunctionReferences.Add(uniqueKey, new List<FunctionReference>());
                }

                details.FunctionReferences[uniqueKey].Add(node.FunctionReference);
            }
        }

        private void ResolveScalarVariableReference(IVariableReference vr)
        {
            // TODO: extend this to UDTs, including member access

            if (vr.VariableReference.Type == VariableReferenceType.System)
            {
                var name = vr.VariableReference.VariableName.TrimStart('@');

                if (!SystemVariableNames.Contains(name))
                {
                    throw NameResolutionError.UnresolvableVariableReference(vr);
                }
            }
            else if (details.VariableReferences.ContainsKey(vr.VariableReference.VariableName))
            {
                vr.VariableReference = details.VariableReferences[vr.VariableReference.VariableName];

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

        private void ResolveColumnReference(CommonTableExpression cte, IEnumerable<TableReference> sourceTables, ColumnContext context, IColumnReference cr)
        {
            // Star columns cannot be resolved, treat them separately
            if (!cr.ColumnReference.IsResolved && !cr.ColumnReference.IsStar && !cr.ColumnReference.IsComplexExpression)
            {
                ColumnReference ncr = null;
                int q = 0;

                if (cr.ColumnReference.ParentTableReference.IsUndefined)
                {
                    // This has an empty table reference (only column name specified)
                    // Look for a match based on column name only
                    foreach (var tr in sourceTables)
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
                    foreach (var ccr in cr.ColumnReference.ParentTableReference.ColumnReferences)
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

                UpdateColumnReference(cr, ncr, context);
            }
        }

        private void UpdateColumnReference(IColumnReference cr, ColumnReference ncr, ColumnContext context)
        {
            // Update column context of the referenced column
            ncr.ColumnContext |= context;

            // Make copy here
            ncr = new ColumnReference(ncr);
            ncr.IsResolved = true;

            if (ncr.ColumnAlias != null)
            {
                // If it is an aliased subquery column, make sure it remains referenced
                // by alias and re-aliasing remains
                ncr.ColumnName = ncr.ColumnAlias;
            }

            // Preserve column alias of referencing column
            ncr.ColumnAlias = cr.ColumnReference.ColumnAlias;

            // Associate column with the resolved reference
            cr.ColumnReference = ncr;
        }

        #endregion
        #region Query constructs

        /// <summary>
        /// Descend into subtree and resolve subqueries bottom-up
        /// </summary>
        /// <param name="node"></param>
        /// <param name="depth"></param>
        private void ResolveSubqueries(CommonTableExpression cte, int depth, QueryContext queryContext, Node node)
        {
            foreach (var n in node.Nodes)
            {
                if (n is Node && !(n is Subquery))
                {
                    switch (n)
                    {
                        case InSemiJoinPredicate p1:
                        case ComparisonSemiJoinPredicate p2:
                        case ExistsSemiJoinPredicate p3:
                            queryContext |= QueryContext.SemiJoin;
                            break;
                        default:
                            break;
                    }

                    ResolveSubqueries(cte, depth, queryContext, (Node)n);
                }

                if (n is Subquery)
                {
                    ResolveSelect(cte, depth + 1, queryContext | QueryContext.Subquery, (Subquery)n);
                }
            }
        }

        protected void ResolveSelect(CommonTableExpression cte, int depth, QueryContext queryContext, ISelect select)
        {
            var qe = select.QueryExpression;
            ResolveQueryExpression(cte, qe, depth, queryContext);

            var orderBy = select.OrderByClause;

            if (orderBy != null)
            {
                var qs = qe.EnumerateQuerySpecifications().FirstOrDefault();
                ResolveOrderByClause(cte, orderBy, qs, queryContext);
            }
        }

        private void ResolveCommonTableExpression(CommonTableExpression cte)
        {
            foreach (var ct in cte.EnumerateCommonTableSpecifications())
            {
                // Because CTEs can reference themselves (i.e. recursive queries) make sure
                // the specification is added to the dictionary before the resolver
                // is called on it
                cte.CommonTableReferences.Add(ct.TableReference.Alias, ct.TableReference);
                ResolveCommonTableSpecification(cte, ct);
            }
        }

        private void ResolveCommonTableSpecification(CommonTableExpression cte, CommonTableSpecification ts)
        {
            var subquery = ts.Subquery;
            ResolveSelect(cte, 1, QueryContext.CommonTableExpression, subquery);
        }

        protected void ResolveQueryExpression(CommonTableExpression cte, QueryExpression qe, int depth, QueryContext queryContext)
        {
            // Resolve the first part of the query expression independently
            // and make sure it's set as ResultsTableReference
            // This is necessary for CTE evaluation which can be recursive

            int q = 0;

            // Resolve query specifications in the FROM clause
            foreach (var qs in qe.EnumerateDescendants<QuerySpecification>())
            {
                ResolveQuerySpecification(cte, qs, depth, queryContext);

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
        protected void ResolveQuerySpecification(CommonTableExpression cte, QuerySpecification qs, int depth, QueryContext queryContext)
        {
            // At this point the table and column references are all parsed
            // from the query but no name resolution and cross-identification
            // of these references have happened yet. After the cross-idenfication
            // routine, the same tables and columns will be tagged by the
            // same TableReference and ColumnReference instances.
            ResolveTableAndColumnReferences(cte, qs, qs, qs, depth, queryContext);

            // Copy resultset columns to the appropriate collection
            CopyResultsColumns(qs);

            // Add default aliases to column expressions in the form of tablealias_columnname
            AssignDefaultColumnAliases(qs, depth != 0, (queryContext & QueryContext.SemiJoin) != 0);
        }

        protected void ResolveTableAndColumnReferences(CommonTableExpression cte, ISourceTableCollection resolvedSourceTables, ISourceTableConsumer sourceTables, Node node, int depth, QueryContext queryContext)
        {
            // First of all, call everything recursively for subqueries. Subqueries
            // can appear within the table sources and in the where clause semi-join
            // expressions
            ResolveSubqueries(cte, depth + 1, queryContext | QueryContext.Subquery, node);

            SubstituteSourceTableDefaults(cte, resolvedSourceTables, sourceTables);
            CollectSourceTableReferences(cte, resolvedSourceTables, sourceTables);

            // Column identifiers can contain table names, aliases or nothing,
            // resolve them now
            ResolveTableReferences(cte, resolvedSourceTables, TableContext.None, ColumnContext.None, node);

            // Substitute SELECT * expressions
            SubstituteStars(node);

            // Resolve variables and column references of each occurance
            ResolveExpressionReferences(cte, resolvedSourceTables, ColumnContext.None, node);
        }

        protected void ResolveTargetTable(CommonTableExpression cte, ISourceTableCollection resolvedSourceTables, ITableReference target)
        {
            SubstituteSourceTableDefaults(cte, resolvedSourceTables, target.TableReference);
            ResolveSourceTableReference(cte, resolvedSourceTables, target.TableReference);
            ResolveTableReference(null, resolvedSourceTables, target, TableContext.Target, ColumnContext.None);
        }

        protected void ResolveOrderByClause(CommonTableExpression cte, OrderByClause orderBy, QuerySpecification firstqs, QueryContext queryContext)
        {
            if (orderBy != null)
            {
                ResolveTableReferences(cte, firstqs, TableContext.None, ColumnContext.OrderBy, orderBy);
                ResolveExpressionReferences(cte, firstqs, ColumnContext.OrderBy, orderBy);
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
        private void CollectSourceTableReferences(CommonTableExpression cte, ISourceTableCollection resolvedSourceTables, ISourceTableConsumer sourceTables)
        {
            // Collect column references from subqueries or load from the database schema

            foreach (var ts in sourceTables.EnumerateSourceTables(false))
            {
                CollectSourceTableReference(cte, resolvedSourceTables, ts);
            }
        }

        private void CollectSourceTableReference(CommonTableExpression cte, ISourceTableCollection resolvedSourceTables, ITableSource ts)
        {
            var tr = ts.TableReference;
            var exportedName = tr.ExportedName;

            // Make sure that table key is used only once
            if (exportedName != null && resolvedSourceTables.ResolvedSourceTableReferences.ContainsKey(exportedName))
            {
                throw NameResolutionError.DuplicateTableAlias(exportedName, tr.Node);
            }

            var ntr = ResolveSourceTableReference(cte, resolvedSourceTables, tr);

            // Save the table in the query specification
            resolvedSourceTables.ResolvedSourceTableReferences.Add(exportedName, ntr);

            // Collect in the global store
            if (ntr.TableContext.HasFlag(TableContext.TableOrView))
            {
                var uniqueKey = ntr.DatabaseObject.UniqueKey;

                if (!details.SourceTableReferences.ContainsKey(uniqueKey))
                {
                    details.SourceTableReferences.Add(uniqueKey, new List<TableReference>());
                }

                details.SourceTableReferences[uniqueKey].Add(ntr);
                ts.UniqueKey = String.Format("{0}_{1}_{2}", uniqueKey, ntr.Alias, details.SourceTableReferences[uniqueKey].Count - 1);
            }
        }

        /// <summary>
        /// Looks up the table among CTE and direct schema objects
        /// </summary>
        /// <param name="cte"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        public TableReference ResolveSourceTableReference(CommonTableExpression cte, ISourceTableCollection resolvedSourceTables, TableReference tr)
        {
            TableReference ntr;

            if (tr.TableContext.HasFlag(TableContext.Variable))
            {
                ntr = new TableReference(details.VariableReferences[tr.VariableName].TableReference);
                ntr.Alias = tr.Alias;
                ntr.TableContext |= TableContext.Variable;
            }
            else if (tr.IsPossiblyAlias && cte != null && cte.CommonTableReferences.ContainsKey(tr.ExportedName))
            {
                // This is a reference to a CTE query

                ntr = new TableReference(cte.CommonTableReferences[tr.DatabaseObjectName]);
                ntr.TableContext |= TableContext.CommonTable;
            }
            else if (tr.IsPossiblyAlias && resolvedSourceTables.ResolvedSourceTableReferences.ContainsKey(tr.ExportedName))
            {
                // This a reference from a target table to an already resolved source table
                // TODO: test this with other cases

                ntr = new TableReference(tr);
            }
            else if (!tr.TableContext.HasFlag(TableContext.Subquery) && !tr.IsComputed)
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

        private void ResolveOutputTable(CommonTableExpression cte, TargetTableSpecification into)
        {
            SubstituteOutputTableDefaults(cte, into.TableReference, TableContext.Into);
            ResolveOutputTableReference(cte, into.TableReference);
        }

        private void ResolveOutputTableReference(CommonTableExpression cte, TableReference tr)
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

            /* TODO: move to query validation
            if (!ds.IsMutable)
            {
                throw NameResolutionError.TargetDatasetReadOnly((ITableReference)tr.Node);
            }
            */

            tr.DatabaseObject = ds.GetObject(tr.DatabaseName, tr.SchemaName, tr.DatabaseObjectName);

            if (tr.DatabaseObject == null)
            {
                tr.DatabaseObject = new Table(ds)
                {
                    DatabaseName = tr.DatabaseName ?? ds.DatabaseName,
                    SchemaName = tr.SchemaName ?? ds.DefaultSchemaName,
                    TableName = tr.DatabaseObjectName,
                };
            }

            // TODO: if it is a new table, consider figuring out the columns from the query

            // Save it to the global store
            var uniqueKey = tr.DatabaseObject.UniqueKey;

            if (!details.OutputTableReferences.ContainsKey(uniqueKey))
            {
                details.OutputTableReferences.Add(uniqueKey, new List<TableReference>());
            }
            else
            {
                throw NameResolutionError.DuplicateOutputTable(tr);
            }

            details.OutputTableReferences[uniqueKey].Add(tr);
        }

        /// <summary>
        /// Resolves all table references of all nodes below a node,
        /// not descending into subqueries
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="n"></param>
        private void ResolveTableReferences(CommonTableExpression cte, ISourceTableCollection resolvedSourceTables, TableContext tableContext, ColumnContext columnContext, Node n)
        {
            tableContext = GetTableContext(n, tableContext);
            columnContext = GetColumnContext(n, columnContext);

            foreach (object o in n.Nodes)
            {
                // Skip the into and clause and subqueries
                if (o is Node && !(o is IntoClause) && !(o is SubqueryTableSource))
                {
                    ResolveTableReferences(cte, resolvedSourceTables, tableContext, columnContext, (Node)o);   // Recursive call
                }
            }

            if (n is ITableReference)
            {
                ResolveTableReference(cte, resolvedSourceTables, (ITableReference)n, tableContext, columnContext);
            }
        }

        /// <summary>
        /// Resolves a table reference to a table listed in SourceTableReferences
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="tr"></param>
        private void ResolveTableReference(CommonTableExpression cte, ISourceTableCollection resolvedSourceTables, ITableReference node, TableContext tableContext, ColumnContext columnContext)
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
                        resolvedSourceTables.ResolvedSourceTableReferences.ContainsKey(node.TableReference.DatabaseObjectName))
                {
                    // if only table name found and that's an alias
                    alias = node.TableReference.DatabaseObjectName;
                }

                if (alias != null)
                {
                    ntr = resolvedSourceTables.ResolvedSourceTableReferences[alias];
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
                    foreach (var tr in resolvedSourceTables.ResolvedSourceTableReferences.Values)
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

                ntr.TableContext |= tableContext;
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
        private void SubstituteStars(Node node)
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
        protected void SubstituteSourceTableDefaults(CommonTableExpression cte, ISourceTableCollection resolvedSourceTables, ISourceTableConsumer sourceTables)
        {
            foreach (var ts in sourceTables.EnumerateSourceTables(false))
            {
                // TODO: any other exclusions?
                if (!ts.IsSubquery && !ts.TableReference.IsComputed)
                {
                    SubstituteSourceTableDefaults(cte, resolvedSourceTables, ts.TableReference);
                }
            }
        }

        protected void SubstituteSourceTableDefaults(CommonTableExpression cte, ISourceTableCollection resolvedSourceTables, TableReference tr)
        {
            try
            {
                if (tr.IsPossiblyAlias &&
                    (cte != null && cte.CommonTableReferences.ContainsKey(tr.DatabaseObjectName) ||
                     resolvedSourceTables != null && resolvedSourceTables.ResolvedSourceTableReferences.ContainsKey(tr.DatabaseObjectName)))
                {
                    // Don't do any substitution if referencing a common table or anything that aliased
                }
                else if (tr.TableContext.HasFlag(TableContext.TableOrView))
                {
                    tr.SubstituteDefaults(SchemaManager, defaultTableDatasetName);
                }
                else if (tr.TableContext.HasFlag(TableContext.UserDefinedFunction))
                {
                    tr.SubstituteDefaults(SchemaManager, defaultFunctionDatasetName);
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw NameResolutionError.UnresolvableDatasetReference(ex, tr);
            }
        }

        private void SubstituteOutputTableDefaults(CommonTableExpression cte, TargetTableSpecification target, TableContext tableContext)
        {
            var tr = target.TableName?.TableReference;
            SubstituteOutputTableDefaults(cte, tr, tableContext);
        }

        private void SubstituteOutputTableDefaults(CommonTableExpression cte, TableReference tr, TableContext tableContext)
        {
            try
            {
                tr.TableContext |= tableContext;
                tr.SubstituteDefaults(SchemaManager, defaultTableDatasetName);
            }
            catch (KeyNotFoundException ex)
            {
                throw NameResolutionError.UnresolvableDatasetReference(ex, tr);
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

        private void SubstituteDataTypeDefaults(DataTypeReference dr)
        {
            if (!dr.IsSystem)
            {
                try
                {
                    dr.SubstituteDefaults(SchemaManager, defaultDataTypeDatasetName);
                }
                catch (KeyNotFoundException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, dr);
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
        private void AssignDefaultColumnAliases(QuerySpecification qs, bool subquery, bool singleColumnSubquery)
        {
            var aliases = new HashSet<string>(SchemaManager.Comparer);

            var cnt = qs.EnumerateSelectListColumnExpressions().Count();

            int q = 0;
            foreach (var ce in qs.EnumerateSelectListColumnExpressions())
            {
                var cr = ce.ColumnReference;
                string alias;

                if (singleColumnSubquery && q > 0)
                {
                    throw NameResolutionError.SingleColumnSubqueryRequired(ce);
                }

                if (cr.ColumnAlias == null)
                {
                    if (cr.ColumnName == null)
                    {
                        if (subquery && !singleColumnSubquery && cnt > 1)
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
                        if (cr.ParentTableReference != null && cr.ParentTableReference.Alias != null)
                        {
                            alias = GetUniqueColumnAlias(aliases, String.Format("{0}_{1}", cr.ParentTableReference.Alias, cr.ColumnName));
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

                q++;
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

        private void ResolveDataTypeReference(DataTypeReference dr)
        {
            if (!dr.IsResolved && dr.IsUserDefined)
            {
                // Load table description from underlying schema
                // Attempt to load dataset and throw exception of name cannot be resolved
                DatasetBase ds;

                try
                {
                    ds = schemaManager.Datasets[dr.DatasetName];
                }
                catch (KeyNotFoundException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, dr);
                }
                catch (SchemaException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, dr);
                }

                // Because this is the base type only, create a copy here since
                // properties like IsNullable will be overwritten later
                var dt = (DataType)ds.GetObject(dr.DatabaseName, dr.SchemaName, dr.DatabaseObjectName);
                dr.DatabaseObject = new DataType(dt);

                // TODO: load data type columns if necessary
            }

            dr.IsResolved = true;
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
