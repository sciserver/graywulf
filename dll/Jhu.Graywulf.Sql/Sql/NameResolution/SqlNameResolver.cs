using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryTraversal;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class SqlNameResolver : SqlQueryVisitorSink
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

        private SqlNameResolverOptions options;
        private SqlQueryVisitor visitor;


        private TokenList memberNameParts;

        // The schema manager is used to resolve identifiers that are not local to the details,
        // i.e. database, table, columns etc. names
        private SchemaManager schemaManager;
        private QueryDetails details;

        #endregion
        #region Properties

        public SqlNameResolverOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        protected SqlQueryVisitor Visitor
        {
            get { return visitor; }
        }

        protected TokenList MemberNameParts
        {
            get { return memberNameParts; }
        }

        /// <summary>
        /// Gets or sets the schema manager to be used by the name resolver
        /// </summary>
        public SchemaManager SchemaManager
        {
            get { return schemaManager; }
            set { schemaManager = value; }
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
            this.options = new SqlNameResolverOptions();
            this.visitor = new SqlQueryVisitor(this)
            {
                Options = new SqlQueryVisitorOptions()
                {
                    LogicalExpressionTraversal = ExpressionTraversalMethod.Infix,
                    ExpressionTraversal = ExpressionTraversalMethod.Infix,
                    VisitExpressionSubqueries = true,
                    VisitPredicateSubqueries = true,
                    VisitSchemaReferences = false
                }
            };
            this.memberNameParts = new TokenList();
            this.schemaManager = null;
            this.details = null;
        }


        #endregion
        #region Main entry points

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
            Visitor.Execute(details.ParsingTree);
            details.IsResolved = true;
        }

        #endregion
        #region Visitor dispatch functions

        protected internal override void AcceptVisitor(SqlQueryVisitor visitor, Token node)
        {
            Accept((dynamic)node);
        }

        protected virtual void Accept(Token node)
        {
            // Default dispatch, do nothing
        }

        #endregion
        #region Qualified identifier visitors

        protected virtual void Accept(SystemVariable node)
        {
            node.VariableReference = ResolveScalarVariableReference(node.VariableReference);
        }

        protected virtual void Accept(UserVariable node)
        {
            if (Visitor.ColumnContext.HasFlag(ColumnContext.Expression) ||
                Visitor.ColumnContext.HasFlag(ColumnContext.SelectList))
            {
                node.VariableReference = ResolveScalarVariableReference(node.VariableReference);
            }
        }

        protected virtual void Accept(ColumnIdentifier node)
        {
            var ncr = ResolveColumnReference(node.ColumnReference);

            if (ncr == null)
            {
                throw NameResolutionError.UnresolvableColumnReference(node.ColumnReference);
            }
            else
            {
                UpdateColumnReference(node.ColumnReference, ncr, Visitor.ColumnContext);
            }
        }

        protected virtual void Accept(FunctionIdentifier node)
        {
            SubstituteFunctionDefaults(node.FunctionReference);
            node.FunctionReference = ResolveFunctionReference(node.FunctionReference);
            CollectFunctionReference(node.FunctionReference);
        }

        protected virtual void Accept(DataTypeIdentifier node)
        {
            SubstituteDataTypeDefaults(node.DataTypeReference);
            node.DataTypeReference = ResolveDataTypeReference(node.DataTypeReference);
            CollectDataTypeReference(node.DataTypeReference);
        }

        protected virtual void Accept(TableOrViewIdentifier node)
        {
            node.TableReference = ResolveTableReference(node.TableReference, null);
        }

        #endregion
        #region Multi-part identifier and UDT member access logic

        // These functions implement column name and UDP property/method binding
        // The logic is the following:
        // - collect member names during the traversal of ObjectName and member access nodes
        // - the end of the chain will be marked with a

        protected virtual void Accept(Operand node)
        {
            // This is where we can figure out if we have a column name or else
            var tokens = visitor.CurrentMemberAccessList;

            ResolveMemberAccessList(node, tokens);
        }

        #endregion
        #region Query node visitors

        protected virtual void Accept(CommonTableSpecification cts)
        {
            Visitor.CommonTableExpression.CommonTableReferences.Add(cts.TableReference.Alias, cts.TableReference);
        }

        protected virtual void Accept(QuerySpecification qs)
        {
            ResolveResultsTableReference(qs);
        }

        protected virtual void Accept(VariableTableSource node)
        {
            node.VariableReference = ResolveTableVariableReference(node.VariableReference);
        }

        protected virtual void Accept(FunctionTableSource node)
        {
            // Branch landing here:
            // - SELECT ... FROM

            node.TableReference.TableContext |= Visitor.TableContext;

            // SELECT ... FROM ...()
            SubstituteFunctionDefaults(node.FunctionReference);
            node.FunctionReference = ResolveFunctionReference(node.FunctionReference);
            CollectFunctionReference(node.FunctionReference);

            throw new NotImplementedException();
            // TODO: review this here, do we have a table reference here?

            CollectSourceTableReference(node.TableReference);
        }

        protected virtual void Accept(SubqueryTableSource node)
        {
            node.TableReference = ResolveTableReference(node.TableReference, null);
        }

        #endregion
        #region Scalar and table-valued variable visitors

        protected virtual void Accept(VariableDeclaration node)
        {
            var vr = node.VariableReference;
            var variable = CreateVariable(vr);
            CollectVariableReference(vr);
        }

        protected virtual void Accept(TableDeclaration node)
        {
            var vr = node.Variable.VariableReference;
            var dr = vr.DataTypeReference;

            ResolveTableDefinition(node.TableDefinition, dr);

            vr.Variable = CreateVariable(vr);
            dr.DataType = CreateDataType(dr);
            CollectVariableReference(vr);
        }

        protected virtual void Accept(CreateTableStatement node)
        {
            node.TableReference = ResolveTableReference(node.TableReference, node.TableDefinition);
        }

        protected virtual void Accept(ColumnDefinition node)
        {
            node.DataTypeIdentifier.DataTypeReference.DataType.IsNullable = node.IsNullable;
        }

        protected virtual void Accept(DropTableStatement node)
        {
            var table = (Schema.Table)node.TargetTable.TableReference.DatabaseObject;

            if (!table.Dataset.Tables.TryRemove(table.UniqueKey, out var t))
            {
                throw NameResolutionError.TableDoesNotExists(node.TargetTable);
            }
        }

        protected virtual void Accept(CreateIndexStatement node)
        {
            // TODO maybe add index to schema?
        }

        #endregion
        #region Schema object creation

        private Schema.Variable CreateVariable(VariableReference vr)
        {
            var variable = new Schema.Variable()
            {
                Name = vr.VariableName,
                DataType = vr.DataTypeReference.DataType,
            };

            return variable;
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

        // TODO: add alter table, create index etc. here

        #endregion
        #region Reference resolution

        protected virtual bool IsSystemFunctionName(string name)
        {
            return SystemFunctionNames.Contains(name);
        }

        private FunctionReference ResolveFunctionReference(FunctionReference fr)
        {
            if (!fr.IsUserDefined)
            {
                if (!IsSystemFunctionName(fr.FunctionName))
                {
                    throw NameResolutionError.UnknownFunctionName(fr);
                }
            }
            else
            {
                // Check if dataset specified and make sure it's valid
                if (fr.DatasetName != null)
                {
                    if (!schemaManager.Datasets.ContainsKey(fr.DatasetName))
                    {
                        throw NameResolutionError.UnresolvableDatasetReference(fr);
                    }
                }

                var ds = schemaManager.Datasets[fr.DatasetName];

                var dbo = ds.GetObject(fr.DatabaseName, fr.SchemaName, fr.DatabaseObjectName);

                if (dbo == null)
                {
                    throw NameResolutionError.UnresolvableFunctionReference(fr);
                }

                fr.DatabaseObject = dbo;
            }

            return fr;
        }

        private VariableReference ResolveScalarVariableReference(VariableReference vr)
        {
            if (!vr.IsUserDefined)
            {
                var name = vr.VariableName.TrimStart('@');

                if (!SystemVariableNames.Contains(name))
                {
                    throw NameResolutionError.UnresolvableVariableReference(vr);
                }
            }
            else if (details.VariableReferences.ContainsKey(vr.VariableName))
            {
                vr = details.VariableReferences[vr.VariableName];

                if (vr.VariableContext != VariableContext.Scalar)
                {
                    throw NameResolutionError.ScalarVariableExpected(vr);
                }
            }
            else
            {
                throw NameResolutionError.UnresolvableVariableReference(vr);
            }

            return vr;
        }

        private DataTypeReference ResolveDataTypeReference(DataTypeReference dr)
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

            return dr;
        }

        private VariableReference ResolveTableVariableReference(VariableReference vr)
        {
            if (details.VariableReferences.ContainsKey(vr.VariableName))
            {
                vr = details.VariableReferences[vr.VariableName];

                if (vr.VariableContext != VariableContext.Table)
                {
                    throw NameResolutionError.TableVariableExpected(vr);
                }
            }
            else
            {
                throw NameResolutionError.UnresolvableVariableReference(vr);
            }

            return vr;
        }

        private ColumnReference ResolveColumnReference(ColumnReference cr)
        {
            var qs = Visitor.CurrentQuerySpecification;
            var stp = qs as ISourceTableProvider ?? Visitor.CurrentStatement as ISourceTableProvider;
            var sourceTables = stp?.SourceTableReferences.Values;
            var ttp = Visitor.CurrentStatement as ITargetTableProvider;
            var targetTable = ttp?.TargetTable.TableReference;

            // Star columns cannot be resolved, treat them separately
            // Also, UPDATE SET ... and INSERT (...) columns must be resolved against the target table

            ColumnReference ncr = null;
            int matches = 0;

            if (Visitor.ColumnContext.HasFlag(ColumnContext.Update) ||
                Visitor.ColumnContext.HasFlag(ColumnContext.Insert))
            {
                ncr = ResolveColumnReference(cr, targetTable, ref matches);

                if (matches == 0)
                {
                    throw NameResolutionError.ColumnNotPartOfTargetTable(cr);
                }
            }
            else if (!cr.IsResolved && !cr.IsStar && !cr.IsComplexExpression)
            {
                if (cr.TableReference == null || cr.TableReference.IsUndefined)
                {
                    // This has an empty table reference (only column name specified),
                    // or column is referenced by a multi-part identifier which needs to be resolved now
                    // Look for a match based on column name only

                    // If we are in a subquery in the where clause or elsewhere outside FROM,
                    // it might be necessary to walk up the query expression stack to find the
                    // table

                    while (sourceTables != null)
                    {
                        // Look into all source tables
                        foreach (var tr in sourceTables)
                        {
                            ncr = ResolveColumnReference(cr, tr, ref matches);
                        }

                        if (matches == 1)
                        {
                            break;
                        }

                        // Try to go one query specification up
                        qs = qs?.ParentQuerySpecification;
                        stp = qs as ISourceTableProvider;
                        sourceTables = stp?.SourceTableReferences.Values;
                    }
                }
                else
                {
                    // This has a table reference already so only check
                    // columns of that particular table

                    // TODO: this might never be hit but it doesn't do any harm for now

                    ncr = ResolveColumnReference(cr, cr.TableReference, ref matches);
                }
            }

            return ncr;
        }

        private ColumnReference ResolveColumnReference(ColumnReference cr, TableReference tr, ref int q)
        {
            ColumnReference ncr = null;

            foreach (var ccr in tr.ColumnReferences)
            {
                if (cr.TryMatch(tr, ccr))
                {
                    if (q != 0)
                    {
                        throw NameResolutionError.AmbigousColumnReference(cr);
                    }

                    ncr = ccr;
                    q++;
                }
            }

            return ncr;
        }

        /// <summary>
        /// Resolves a table reference to a table listed in SourceTableReferences
        /// </summary>
        /// <param name="qs"></param>
        /// <param name="tr"></param>
        private TableReference ResolveColumnTableReference(ISourceTableProvider resolvedSourceTables, TableReference tr)
        {
            // Try to resolve the table alias part of a table reference
            // If and alias or table name is specified, this can be done based on
            // the already collected table sources.
            // If no table or alias is specified and the current node is a column reference,
            // where the column is not a complex expression, resolution might be successful by
            // column name only.

            TableReference ntr = null;

            // TODO: add support for variables

            if (tr != null && !tr.IsUndefined && !tr.IsResolved)
            {
                
                string alias = null;

                if (tr.Alias != null)
                {
                    // If table alias found explicitly
                    // TODO: this might never happen since by default the last part before the .* is
                    // put into DatabaseObjectName by default.
                    alias = tr.Alias;
                }
                else if (tr.DatasetName == null &&
                        tr.DatabaseName == null &&
                        tr.SchemaName == null &&
                        tr.DatabaseObjectName != null &&
                        resolvedSourceTables.SourceTableReferences.ContainsKey(tr.DatabaseObjectName))
                {
                    // if only table name found and that's an alias
                    alias = tr.DatabaseObjectName;
                }

                if (alias != null)
                {
                    ntr = resolvedSourceTables.SourceTableReferences[alias];
                }
                else
                {
                    // Check if dataset specified and make sure it's valid
                    if (tr.DatasetName != null)
                    {
                        if (!schemaManager.Datasets.ContainsKey(tr.DatasetName))
                        {
                            throw NameResolutionError.UnresolvableDatasetReference(tr);
                        }
                    }

                    // if only a table name found and that's not an alias -> must be a table
                    int q = 0;
                    foreach (var ttr in resolvedSourceTables.SourceTableReferences.Values)
                    {
                        if (ttr.TryMatch(tr))
                        {
                            if (q != 0)
                            {
                                throw NameResolutionError.AmbigousTableReference(tr);
                            }

                            ntr = ttr;
                            q++;
                        }
                    }
                }

                if (ntr == null)
                {
                    throw NameResolutionError.UnresolvableTableReference(tr);
                }

                ntr.TableContext |= Visitor.TableContext;
                ntr.IsResolved = true;
            }

            // If we are inside a table hint, make sure the reference is to the current table
            if (Visitor.ColumnContext == ColumnContext.Hint)
            {
                // In this case a column reference appears inside a table hint (WITH clause)
                // If the table reference is undefined it must refer to the table itself
                // otherwise we must make sure it is indeed referencing the table itself

                var ts = tr.Node.FindAscendant<SimpleTableSource>();

                if (tr.IsUndefined)
                {
                    return ts.TableReference;
                }
                else if (!tr.TryMatch(ts.TableReference))
                {
                    throw NameResolutionError.DifferentTableReferenceInHintNotAllowed(tr);
                }
            }

            return ntr;
        }

        private void UpdateColumnReference(ColumnReference cr, ColumnReference ncr, ColumnContext context)
        {
            // Update column context of the referenced column
            ncr.ColumnContext |= context;

            cr.IsResolved = true;
            cr.TableReference = ncr.TableReference;
            cr.ParentDataTypeReference = ncr.ParentDataTypeReference;
        }

        protected TableReference ResolveTableReference(TableReference tr, TableDefinition td)
        {
            TableReference ntr = null;

            var sourceTableCollection =
                Visitor.CurrentQuerySpecification as ISourceTableProvider ??
                Visitor.CurrentStatement as ISourceTableProvider;

            var targetTableProvider =
                Visitor.CurrentStatement as ITargetTableProvider;

            // Set it on the original reference, later will be set on the resolved one too
            tr.TableContext |= Visitor.TableContext;

            if ((Visitor.TableContext & TableContext.Output) != 0)
            {
                // Case 1: output table not already in schema
                // SELECT INTO or CREATE TABLE
                // Table is resolved in dedicated callback
                SubstituteOutputTableDefaults(tr);

                if (td != null)
                {
                    // TODO: do we know the columns of the SELECT INTO here?
                    // names are known, but types are not
                    ntr = ResolveTableDefinition(td, tr);
                }
                else
                {
                    ntr = ResolveOutputTableReference(Visitor.CurrentQuerySpecification, tr);
                }

                CollectOutputTableReference(targetTableProvider, tr);
            }
            else if ((Visitor.TableContext & TableContext.Target) != 0)
            {
                // Case 2: target table already in schema
                // INSERT, UPDATE, DELETE, etc.
                SubstituteSourceTableDefaults(sourceTableCollection, tr);
                ntr = ResolveSourceTableReference(sourceTableCollection, tr);
                CollectTargetTableReference(targetTableProvider, ntr);

                // For statement with query parts target table can also
                // appear as a source table for column resolution
                CollectSourceTableReference(ntr);
            }
            else if ((Visitor.TableContext & TableContext.Subquery) != 0)
            {
                // Case 3: aliased subquery
                // SELECT ... FROM (SELECT ...) sq
                // Everything is resolved, just copy to source tables
                CollectSourceTableReference(tr);
            }
            else if ((Visitor.TableContext & TableContext.From) != 0)
            {
                // Case 4: simple source table
                // SELECT ... FROM
                SubstituteSourceTableDefaults(sourceTableCollection, tr);
                ntr = ResolveSourceTableReference(sourceTableCollection, tr);
                CollectSourceTableReference(ntr);
            }
            else
            {
                // Case 4: Table reference in from of SELECT ....*
                // Source table (table.* syntax only)
                SubstituteSourceTableDefaults(sourceTableCollection, tr);
                ntr = ResolveColumnTableReference(sourceTableCollection, tr);
            }

            return ntr;
        }

        private TableReference ResolveOutputTableReference(QuerySpecification qs, TableReference tr)
        {
            tr.CopyColumnReferences(qs.ResultsTableReference.ColumnReferences);
            tr.DatabaseObject = CreateTable(tr);
            tr.TableContext |= Visitor.TableContext;

            return tr;
        }

        private TableReference ResolveTableDefinition(TableDefinition td, TableReference tr)
        {
            foreach (var item in td.TableDefinitionList.EnumerateTableDefinitionItems())
            {
                var cd = item.ColumnDefinition;
                var tc = item.TableConstraint;
                var ti = item.TableIndex;

                if (cd != null)
                {
                    var ncr = new ColumnReference(tr, null, cd.ColumnReference, cd.DataTypeIdentifier.DataTypeReference);
                    if (tr != null)
                    {
                        tr.ColumnReferences.Add(ncr);
                    }
                }

                if (tc != null)
                {
                    // TODO
                }

                if (ti != null)
                {
                    // TODO
                }
            }

            tr.DatabaseObject = CreateTable(tr);

            return tr;
        }

        private void ResolveTableDefinition(TableDefinition td, DataTypeReference dr)
        {
            foreach (var item in td.TableDefinitionList.EnumerateTableDefinitionItems())
            {
                var cd = item.ColumnDefinition;

                if (cd != null)
                {
                    var ncr = new ColumnReference(null, dr, cd.ColumnReference, cd.DataTypeIdentifier.DataTypeReference);
                    if (dr != null)
                    {
                        dr.ColumnReferences.Add(ncr);
                    }
                }
            }

            dr.DataType = CreateDataType(dr);
        }

        /// <summary>
        /// Looks up the table among CTE and table sources and direct schema objects
        /// </summary>
        /// <param name="cte"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        public TableReference ResolveSourceTableReference(ISourceTableProvider resolvedSourceTables, TableReference tr)
        {
            TableReference ntr;

            if (tr.TableContext.HasFlag(TableContext.Variable))
            {
                tr.VariableReference = details.VariableReferences[tr.VariableName];
                tr.LoadColumnReferences(null);
                ntr = tr;
            }
            else if (tr.TableContext.HasFlag(TableContext.Subquery))
            {
                ntr = tr;
                ntr.TableContext |= TableContext.Subquery;
            }
            else if (tr.IsPossiblyAlias &&
                Visitor.CommonTableExpression != null &&
                Visitor.CommonTableExpression.CommonTableReferences.ContainsKey(tr.TableName))
            {
                // This is a reference to a CTE query
                ntr = Visitor.CommonTableExpression.CommonTableReferences[tr.DatabaseObjectName];
            }
            else if (tr.IsPossiblyAlias && resolvedSourceTables.SourceTableReferences.ContainsKey(tr.TableName))
            {
                // This a reference from a target table to an already resolved source table
                // This happens with UPDATE etc.
                ntr = resolvedSourceTables.SourceTableReferences[tr.TableName];
            }
            else if (!tr.IsComputed)
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

                if (ntr.DatabaseObject is TableOrView)
                {
                    ntr.TableContext |= TableContext.TableOrView;
                }

                // Load column descriptions for the table
                ntr.LoadColumnReferences(schemaManager);
            }
            else
            {
                throw new NotImplementedException();
            }

            ntr.IsResolved = true;
            ntr.TableContext |= Visitor.TableContext;

            return ntr;
        }

        private void ResolveMemberAccessList(Operand operand, TokenList tokens)
        {
            // Name resolution goes as follows
            // 1. First try to resolve member access list as a column identifier then
            //    as a scalar function
            // 2. Columns names are in the form of
            //    - column
            //    - schema.column
            //    - database.schema.column
            // 3. Function names are always in the form of
            //    - schema.function
            // 4. Try both column and function resolution; if both match the name is ambiguos
            // 5. Everything else after the resolved part is property and method calls

            // Find the last token in the list which can still be a column name
            // The first token can always be a column name
            int lastcolpart = 0;
            for (int i = 1; i < tokens.Count; i++)
            {
                if (!(tokens[i] is MemberAccess))
                {
                    break;
                }
                else
                {
                    lastcolpart = i;
                }
            }

            ColumnReference cr = null;
            int matchcolpart;
            int matches = 0;

            for (int i = 0; i <= lastcolpart; i++)
            {
                var ncr = ResolverMemberAccessListAsColumn(tokens, i);

                if (ncr != null)
                {
                    matchcolpart = i;
                    matches++;

                    if (matches > 1)
                    {
                        throw NameResolutionError.AmbigousColumnReference(ncr);
                    }

                    cr = ncr;
                }
            }

            // TODO: try to resolve as function call

            if (cr != null)
            {
                // matchcolpart now tells how many of the tokens describe the column
                // exchange these with a single ColumnIdentifier and the rest is all
                // properties and method calls.

                var ci = ColumnIdentifier.Create(cr);
                ((ObjectName)tokens[0]).ReplaceWith(ci);

                // TODO: iterate through the rest and create properties / method calls
            }
        }

        private ColumnReference ResolverMemberAccessListAsColumn(TokenList tokens, int colpart)
        {
            ColumnReference ncr;
            var tr = TableReference.Interpret(tokens, colpart);
            var cr = ColumnReference.Interpret(tokens, colpart);

            if (tr == null)
            {
                 ncr = ResolveColumnReference(cr);
            }
            else
            {
                // TODO: consider pushing it down to column resolver function
                var sourceTableCollection =
                    Visitor.CurrentQuerySpecification as ISourceTableProvider ??
                    Visitor.CurrentStatement as ISourceTableProvider;

                tr = ResolveSourceTableReference(sourceTableCollection, tr);
                cr.TableReference = tr;
                ncr = ResolveColumnReference(cr);
            }

            return ncr;
        }

        #endregion
        #region Default substitution logic

        /// <summary>
        /// Substitutes table defaults for all tables that are supposed to exist during query execution
        /// </summary>
        /// <param name="resolvedSourceTables"></param>
        /// <param name="tr"></param>
        protected void SubstituteSourceTableDefaults(ISourceTableProvider resolvedSourceTables, TableReference tr)
        {
            try
            {
                if (tr.IsPossiblyAlias &&
                    (Visitor.CommonTableExpression != null && Visitor.CommonTableExpression.CommonTableReferences.ContainsKey(tr.DatabaseObjectName) ||
                     resolvedSourceTables != null && resolvedSourceTables.SourceTableReferences.ContainsKey(tr.DatabaseObjectName)))
                {
                    // Don't do any substitution if referencing a common table or anything that aliased
                }
                else if (tr.TableContext.HasFlag(TableContext.UserDefinedFunction))
                {
                    tr.SubstituteDefaults(SchemaManager, options.DefaultFunctionDatasetName);
                }
                else if (tr.TableContext.HasFlag(TableContext.Variable))
                {
                    // No substitution
                }
                else
                {
                    tr.SubstituteDefaults(SchemaManager, options.DefaultTableDatasetName);
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw NameResolutionError.UnresolvableDatasetReference(ex, tr);
            }
        }

        /// <summary>
        /// Substitutes table default for tables that are created during query execution 
        /// </summary>
        /// <param name="tr"></param>
        private void SubstituteOutputTableDefaults(TableReference tr)
        {
            try
            {
                tr.SubstituteDefaults(SchemaManager, options.DefaultOutputDatasetName);
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
        private void SubstituteFunctionDefaults(FunctionReference fr)
        {
            if (!fr.IsSystem)
            {
                try
                {
                    fr.SubstituteDefaults(SchemaManager, options.DefaultFunctionDatasetName);
                }
                catch (KeyNotFoundException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, fr);
                }
            }
        }

        private void SubstituteDataTypeDefaults(DataTypeReference dr)
        {
            if (!dr.IsSystem)
            {
                try
                {
                    dr.SubstituteDefaults(SchemaManager, options.DefaultDataTypeDatasetName);
                }
                catch (KeyNotFoundException ex)
                {
                    throw NameResolutionError.UnresolvableDatasetReference(ex, dr);
                }
            }
        }

        #endregion
        #region Collect tables, functions and variables

        private void CollectFunctionReference(FunctionReference fr)
        {
            if (fr.DatabaseObject != null)
            {
                var uniqueKey = fr.DatabaseObject.UniqueKey;

                if (!details.FunctionReferences.ContainsKey(uniqueKey))
                {
                    details.FunctionReferences.Add(uniqueKey, new List<FunctionReference>());
                }

                details.FunctionReferences[uniqueKey].Add(fr);
            }
        }

        private void CollectVariableReference(VariableReference vr)
        {
            var uniquekey = vr.VariableName;
            if (!details.VariableReferences.ContainsKey(uniquekey))
            {
                details.VariableReferences.Add(uniquekey, vr);
            }
            else
            {
                throw NameResolutionError.DuplicateVariableName(vr);
            }
        }

        private void CollectDataTypeReference(DataTypeReference dr)
        {
            var uniquekey = dr.DatabaseObject.UniqueKey;
            if (!details.DataTypeReferences.ContainsKey(uniquekey))
            {
                details.DataTypeReferences.Add(uniquekey, dr);
            }
        }

        private void CollectSourceTableReference(TableReference tr)
        {
            var stp = Visitor.CurrentQuerySpecification as ISourceTableProvider ??
                      Visitor.CurrentStatement as ISourceTableProvider;

            // Store table source on query specification or statement level
            if (stp != null)
            {
                CollectSourceTableReference(stp, tr, false);
            }

            // If it is a table, save to the global store also, and generate unique name
            if (tr.DatabaseObject is TableOrView)
            {
                // Collect in the global store
                var uniquekey = tr.DatabaseObject.UniqueKey;

                if (!details.SourceTableReferences.ContainsKey(uniquekey))
                {
                    details.SourceTableReferences.Add(uniquekey, new List<TableReference>());
                }

                details.SourceTableReferences[uniquekey].Add(tr);

                if (tr.TableSource != null)
                {
                    tr.TableSource.UniqueKey = String.Format("{0}_{1}_{2}", uniquekey, tr.Alias, details.SourceTableReferences[uniquekey].Count - 1);
                }
            }
        }

        private void CollectSourceTableReference(ISourceTableProvider stp, TableReference tr, bool allowDuplicates)
        {
            var sourceTables = stp.SourceTableReferences;
            string exportedName = tr.Alias ?? tr.VariableName ?? tr.TableName;

            if (exportedName == null)
            {
                throw new InvalidOperationException();
            }

            // The target table is added to the collection only if it doesn't appear already
            // in the list. The tricky part is that we have to check on the database object level
            // because the source table might be aliased. Also, do not add the target table again
            // if it is a reference to a source table by alias.
            if ((tr.TableContext & TableContext.Target) != 0)
            {
                foreach (var tt in sourceTables.Values)
                {
                    if (tt.TableContext.HasFlag(TableContext.TableOrView) &&
                        tt.DatabaseObject.UniqueKey == tr.DatabaseObject.UniqueKey)
                    {
                        return;
                    }
                }

                if (sourceTables.ContainsKey(exportedName) && (tr.TableContext & TableContext.Target) != 0)
                {
                    return;
                }
            }

            if (!allowDuplicates && sourceTables.ContainsKey(exportedName))
            {
                throw NameResolutionError.DuplicateTableAlias(exportedName, tr.Node);
            }

            if (!sourceTables.ContainsKey(exportedName))
            {
                // Save the table in the query specification
                sourceTables.Add(exportedName, tr);
            }
        }

        private void CollectTargetTableReference(ITargetTableProvider targetTable, TableReference tr)
        {
            var uniqueKey = tr.DatabaseObject.UniqueKey;

            if (!details.TargetTableReferences.ContainsKey(uniqueKey))
            {
                details.TargetTableReferences.Add(uniqueKey, new List<TableReference>());
            }

            details.TargetTableReferences[uniqueKey].Add(tr);
        }

        private void CollectOutputTableReference(ITargetTableProvider targetTable, TableReference tr)
        {
            var table = (Table)tr.DatabaseObject;
            var uniqueKey = table.UniqueKey;

            // Add to schema
            if (!table.Dataset.Tables.TryAdd(uniqueKey, table))
            {
                throw NameResolutionError.TableAlreadyExists(tr.Node);
            }

            // Add to query details
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

        protected void ResolveResultsTableReference(QuerySpecification qs)
        {
            if (Visitor.QueryContext.HasFlag(QueryContext.Subquery))
            {
                qs.ResultsTableReference.TableContext |= TableContext.Subquery;
            }

            // ColumnExpression are rendered with original table name by the code generator
            // but are referenced by outer queries by table alias. For this reason, at
            // this point we need to make a copy of all column references and update the
            // table reference
            CopyResultTableColumns(qs);
        }

        protected void CopyResultTableColumns(QuerySpecification qs)
        {
            var tr = qs.ResultsTableReference;
            int index = 0;
            foreach (var ce in qs.SelectList.EnumerateColumnExpressions())
            {
                var cr = ce.ColumnReference;

                if (cr.IsStar)
                {
                    if (cr.TableReference.IsUndefined)
                    {
                        foreach (var ts in qs.SourceTableReferences.Values)
                        {
                            foreach (var ccr in ts.ColumnReferences)
                            {
                                CopyResultTableColumn(ccr, tr, index++);
                            }
                        }
                    }
                    else
                    {
                        var ts = qs.SourceTableReferences[cr.TableReference.Alias ?? cr.TableReference.TableName];
                        foreach (var ccr in ts.ColumnReferences)
                        {
                            CopyResultTableColumn(ccr, tr, index++);
                        }
                    }
                }
                else
                {
                    CopyResultTableColumn(cr, tr, index++);
                }
            }
        }

        protected ColumnReference CopyResultTableColumn(ColumnReference cr, TableReference tr, int index)
        {
            cr.ColumnContext |= ColumnContext.SelectList;
            var ncr = new ColumnReference(tr, cr)
            {
                SelectListIndex = index,
                ColumnAlias = null,
                ColumnName = cr.ColumnAlias ?? cr.ColumnName
            };
            tr.ColumnReferences.Add(ncr);
            return ncr;
        }

        #endregion
    }
}
