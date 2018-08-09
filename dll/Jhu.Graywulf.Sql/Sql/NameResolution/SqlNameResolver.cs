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
        #region Private member variables

        private SqlNameResolverOptions options;
        private DatasetBase dataset;
        private SqlQueryVisitor visitor;

        // These are used to collect tokens during name resolution
        private TokenList memberNameParts;
        private List<ColumnReference> columnDefinitionReferences;

        // The schema manager is used to resolve identifiers that are not local to the details,
        // i.e. database, table, columns etc. names
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

        public DatasetBase Dataset
        {
            get { return dataset; }
            set { dataset = value; }
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
            this.options = CreateOptions();
            this.visitor = CreateVisitor();

            this.memberNameParts = null;
            this.columnDefinitionReferences = null;

            this.details = null;
        }

        protected virtual SqlNameResolverOptions CreateOptions()
        {
            return new SqlNameResolverOptions();
        }

        protected virtual SqlQueryVisitor CreateVisitor()
        {
            return new SqlQueryVisitor(this)
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

        public virtual void Execute(QueryDetails details)
        {
            this.details = details;
            this.memberNameParts = new TokenList();
            this.columnDefinitionReferences = new List<ColumnReference>();

            Visitor.Execute(details.ParsingTree);

            details.IsResolved = true;
            this.memberNameParts = null;
            this.columnDefinitionReferences = null;
        }

        #endregion
        #region Visitor dispatch functions

        protected internal override void AcceptVisitor(SqlQueryVisitor visitor, Token node)
        {
            Accept((dynamic)node);
        }

        protected internal override void AcceptVisitor(SqlQueryVisitor visitor, IDatabaseObjectReference node)
        {
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
            else if (Visitor.TableContext.HasFlag(TableContext.Target))
            {
                node.VariableReference = ResolveTableVariableReference(node.VariableReference);
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

        protected virtual void Accept(ColumnDefinition node)
        {
            if (visitor.Pass == 0)
            {
                columnDefinitionReferences.Add(node.ColumnReference);
            }
        }

        protected virtual void Accept(ColumnNullSpecification node)
        {
            var cr = columnDefinitionReferences[columnDefinitionReferences.Count - 1];
            cr.DataTypeReference.DataType.IsNullable = node.IsNullable;
        }

        protected virtual void Accept(ColumnExpression node)
        {
            node.ColumnReference = ColumnReference.Interpret(node);
        }

        protected virtual void Accept(StarColumnIdentifier node)
        {
            // Only attempt to resolve table reference if it is defined in the query
            if (!node.ColumnReference.TableReference.IsUndefined)
            {
                node.ColumnReference.TableReference = ResolveTableReference(node.ColumnReference.TableReference, null);
            }
        }

        protected virtual void Accept(FunctionIdentifier node)
        {
            SubstituteFunctionDefaults(node.FunctionReference);
            node.FunctionReference =
                ResolveFunctionReference(node.FunctionReference) ??
                throw NameResolutionError.UnresolvableFunctionReference(node.FunctionReference);
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
            if (node.Stack.First is ObjectName)
            {
                // This is where we can figure out if we have a column name or else
                ResolveObjectNameAndMemberAccessList(node);
            }
            else
            {
                // These are just property and method calls, run a simple substitution
                ResolveMemberAccessList(node, 0);
            }
        }

        #endregion
        #region Query node visitors

        protected virtual void Accept(CommonTableSpecification cts)
        {
            if (visitor.Pass == 0)
            {
                Visitor.CommonTableExpression.CommonTableReferences.Add(cts.TableReference.Alias, cts.TableReference);
            }
        }

        protected virtual void Accept(QuerySpecification qs)
        {
            switch (Visitor.Pass)
            {
                case 0:
                    qs.ParentQuerySpecification = Visitor.CurrentQuerySpecification;
                    break;
                case 1:
                    ResolveResultsTableReference(qs);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void Accept(VariableTableSource node)
        {
            node.VariableReference = ResolveTableVariableReference(node.VariableReference);
            node.TableReference = ResolveTableReference(node.TableReference, null);
        }

        protected virtual void Accept(FunctionTableSource node)
        {
            // Branch landing here:
            // - SELECT ... FROM

            node.TableReference.DatabaseObject = node.FunctionReference.DatabaseObject;
            node.TableReference.LoadColumnReferences();

            /*
             * TODO: only OPENROWSET etc. and VALUES constructs can have alias lists!
             * move this elsewhere
            // The function call can also have an alias list
            List<ColumnAlias> calist = null;
            var cal = node.FindDescendant<ColumnAliasList>();
            if (cal != null)
            {
                calist = new List<ColumnAlias>(cal.EnumerateDescendants<ColumnAlias>());

                int q = 0;
                foreach (var cr in node.TableReference.ColumnReferences)
                {
                    // if column alias list is present, use the alias instead of the original name
                    cr.ColumnName = ReferenceBase.RemoveIdentifierQuotes(calist[q].Value);
                    q++;
                }
            }
            */

            node.TableReference.TableContext |= Visitor.TableContext | TableContext.UserDefinedFunction;
            CollectSourceTableReference(node.TableReference);
        }

        protected virtual void Accept(SubqueryTableSource node)
        {
            // This is fishy here
            node.TableReference = ResolveTableReference(node.TableReference, null);
        }

        protected virtual void Accept(TargetTableSpecification node)
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
            var ds = LoadDataset(tr);

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

        protected virtual DatasetBase LoadDataset(DatabaseObjectReference dr)
        {
            return dataset;
        }

        protected virtual void LoadDatabaseObject(DatabaseObjectReference dr)
        {
            var ds = LoadDataset(dr);
            dr.LoadDatabaseObject(ds);
        }

        protected virtual bool IsSystemFunctionName(string name)
        {
            return Constants.SystemFunctionNames.Contains(name);
        }

        protected virtual bool IsSystemVariableName(string name)
        {
            return Constants.SystemVariableNames.Contains(name);
        }

        private FunctionReference ResolveFunctionReference(FunctionReference fr)
        {
            // TODO: scalar CLR function calls must have the schema name specified
            // but table valued functions don't

            if (String.IsNullOrWhiteSpace(fr.SchemaName) && IsSystemFunctionName(fr.FunctionName))
            {
                fr.IsSystem = true;
            }
            else
            {
                fr.IsUserDefined = true;
                LoadDatabaseObject(fr);

                if (fr.DatabaseObject == null)
                {
                    return null;
                }
            }

            return fr;
        }

        private VariableReference ResolveScalarVariableReference(VariableReference vr)
        {
            if (!vr.IsUserDefined)
            {
                var name = vr.VariableName.TrimStart('@');

                if (!IsSystemVariableName(name))
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
                LoadDatabaseObject(dr);

                if (dr.DatabaseObject == null)
                {
                    return null;
                }

                // Reset nullable because we will use this for actual column nullability
                // instead of the data type itself supporting null values
                dr.DataType.IsNullable = false;

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
            var targetTable = ttp?.TargetTableReference;

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
                            ncr = ResolveColumnReference(cr, tr, ref matches) ?? ncr;
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

        protected TableReference FindTableByAlias(TableReference tr, bool excludeOuterQueries)
        {
            var targetTableProvider =
                Visitor.CurrentStatement as ITargetTableProvider;

            var alias = tr.TableName ?? tr.Alias;

            if (targetTableProvider != null && SchemaManager.Comparer.Compare(targetTableProvider.TargetTableReference.Alias, alias) == 0)
            {
                // TODO: Can this happen? Can target table be aliased?
                return targetTableProvider.TargetTableReference;
            }
            else if (Visitor.CommonTableExpression != null && Visitor.CommonTableExpression.CommonTableReferences.ContainsKey(alias))
            {
                return Visitor.CommonTableExpression.CommonTableReferences[alias];
            }
            else
            {
                var qs = Visitor.CurrentQuerySpecification;
                var sourceTables =
                    qs?.SourceTableReferences ??
                    Visitor.CurrentStatement.SourceTableReferences;

                while (sourceTables != null)
                {
                    if (sourceTables.ContainsKey(alias))
                    {
                        return sourceTables[alias];
                    }
                    else if (!excludeOuterQueries && qs != null)
                    {
                        // Try to go one query specification up
                        qs = qs?.ParentQuerySpecification;
                        sourceTables = qs?.SourceTableReferences;
                    }
                    else if (sourceTables != Visitor.CurrentStatement.SourceTableReferences)
                    {
                        sourceTables = Visitor.CurrentStatement.SourceTableReferences;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        protected TableReference ResolveTableReference(TableReference tr, TableDefinition td)
        {
            // Set it on the original reference, later will be set on the resolved one too
            tr.TableContext |= Visitor.TableContext;

            if (tr.IsResolved)
            {
                return tr;
            }
            else
            {
                TableReference ntr = null;

                var sourceTableCollection =
                    Visitor.CurrentQuerySpecification as ISourceTableProvider ??
                    Visitor.CurrentStatement as ISourceTableProvider;

                var targetTableProvider =
                    Visitor.CurrentStatement as ITargetTableProvider;

                if (Visitor.TableContext.HasFlag(TableContext.Create))
                {
                    // Case 1: output table not already in schema
                    // DECLARE .. AS TABLE and CREATE TABLE

                    if (td == null)
                    {
                        // First pass
                        SubstituteOutputTableDefaults(tr);
                        ntr = tr;
                    }
                    else
                    {
                        // Second pass
                        ntr = ResolveTableDefinition(td, tr);
                        CollectOutputTableReference(targetTableProvider, tr);
                    }
                }
                else if (Visitor.TableContext.HasFlag(TableContext.Into))
                {
                    // Case 2: output table not already in schema
                    // SELECT INTO

                    SubstituteOutputTableDefaults(tr);
                    ntr = ResolveOutputTableReference(Visitor.CurrentQuerySpecification, tr);
                    CollectOutputTableReference(targetTableProvider, tr);
                }
                else if ((Visitor.TableContext & TableContext.Target) != 0)
                {
                    // Case 3: target table already in schema
                    // INSERT, UPDATE, DELETE, etc.
                    SubstituteSourceTableDefaults(sourceTableCollection, tr, true);
                    ntr = ResolveSourceTableReference(sourceTableCollection, tr, true);
                    CollectTargetTableReference(targetTableProvider, ntr);

                    // For statement with query parts target table can also
                    // appear as a source table for column resolution
                    CollectSourceTableReference(ntr);
                }
                else if ((Visitor.TableContext & TableContext.Subquery) != 0)
                {
                    // Case 4: aliased subquery
                    // SELECT ... FROM (SELECT ...) sq
                    // Everything is resolved, just copy to source tables
                    ntr = tr;
                    CollectSourceTableReference(tr);
                }
                else if ((Visitor.TableContext & TableContext.From) != 0)
                {
                    // Case 5: simple source table
                    // SELECT ... FROM
                    SubstituteSourceTableDefaults(sourceTableCollection, tr, true);
                    ntr = ResolveSourceTableReference(sourceTableCollection, tr, true);

                    if (ntr == null)
                    {
                        throw NameResolutionError.UnresolvableTableReference(tr);
                    }

                    CollectSourceTableReference(ntr);
                }
                else
                {
                    // Case 6: Table reference in from of SELECT ....*
                    // Source table (table.* and table.columnname syntax only)
                    SubstituteSourceTableDefaults(sourceTableCollection, tr, false);
                    ntr = ResolveColumnTableReference(sourceTableCollection, tr);
                }

                return ntr;
            }
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
            foreach (var cr in columnDefinitionReferences)
            {
                var ncr = new ColumnReference(tr, null, cr, cr.DataTypeReference);
                tr.ColumnReferences.Add(ncr.ColumnName, ncr);
            }

            foreach (var tc in td.EnumerateDescendantsRecursive<TableConstraint>())
            {
                // TODO
            }

            foreach (var tc in td.EnumerateDescendantsRecursive<TableIndex>())
            {
                // TODO
            }

            tr.DatabaseObject = CreateTable(tr);

            columnDefinitionReferences.Clear();

            return tr;
        }

        private DataTypeReference ResolveTableDefinition(TableDefinition td, DataTypeReference dr)
        {
            foreach (var cr in columnDefinitionReferences)
            {
                var ncr = new ColumnReference(null, dr, cr, cr.DataTypeReference);
                dr.ColumnReferences.Add(ncr.ColumnName, ncr);
            }

            dr.DataType = CreateDataType(dr);

            columnDefinitionReferences.Clear();

            return dr;
        }

        /// <summary>
        /// Looks up the table among CTE and table sources and direct schema objects
        /// </summary>
        /// <param name="cte"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        public TableReference ResolveSourceTableReference(ISourceTableProvider resolvedSourceTables, TableReference tr, bool excludeOuterQueries)
        {
            TableReference ntr;

            if (tr.TableContext.HasFlag(TableContext.Variable))
            {
                tr.VariableReference = details.VariableReferences[tr.VariableName];
                tr.LoadColumnReferences();
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
            else if (tr.IsPossiblyAlias && (ntr = FindTableByAlias(tr, excludeOuterQueries)) != null)
            {
                // This a reference from a target table to an already resolved source table
                // This happens with UPDATE etc. and subqueries referencing outer query specifications
            }
            else if (!tr.IsComputed)
            {
                // This is a direct reference to a table or a view but not to a function or subquery
                // Load object from the schema
                ntr = tr;

                LoadDatabaseObject(ntr);
                if (ntr.DatabaseObject == null)
                {
                    return null;
                }

                ntr.LoadColumnReferences();
            }
            else
            {
                throw new NotImplementedException();
            }

            if (ntr != null)
            {
                ntr.IsResolved = true;
                ntr.TableContext |= Visitor.TableContext;
            }

            return ntr;
        }

        private void ResolveObjectNameAndMemberAccessList(Operand operand)
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

            var tokens = visitor.CurrentMemberAccessList;

            var cr = ResolveMemberAccessListAsColumn(out int matchcolpart);
            var fr = ResolveMemberAccessListAsFunction(out int matchfunpart);

            if (cr != null && fr != null)
            {
                throw NameResolutionError.AmbigousColumnOrFunctionReference(operand);
            }
            else if (cr != null)
            {
                // matchcolpart now tells how many of the tokens describe the column
                // exchange these with a single ColumnIdentifier and the rest is all
                // properties and method calls.

                cr.ColumnContext |= visitor.ColumnContext;

                var ci = ColumnIdentifier.Create(cr, matchcolpart);
                ((ObjectName)tokens[0]).ReplaceWith(ci);
                ResolveMemberAccessList(operand, matchcolpart);
            }
            else if (fr != null)
            {
                var args = ((MemberCall)tokens[matchfunpart]).GetArguments();
                var fc = ScalarFunctionCall.Create(fr, args);
                ((ObjectName)tokens[0]).ReplaceWith(fc);
                CollectFunctionReference(fr);
                ResolveMemberAccessList(operand, matchfunpart);
            }
            else
            {
                throw NameResolutionError.UnresolvableMemberAccessList(operand);
            }
        }

        private ColumnReference ResolveMemberAccessListAsColumn(out int matchcolpart)
        {
            var tokens = visitor.CurrentMemberAccessList;

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

            // Columns names can at most be in the form of database.schema.table.column
            if (lastcolpart > 3)
            {
                lastcolpart = 3;
            }

            //ColumnReference cr = null;
            //int matches = 0;
            matchcolpart = 0;

            // Try to match longest first
            for (int i = lastcolpart; 0 <= i; i--)
            {
                var ncr = ResolveMemberAccessListAsColumn(i);

                if (ncr != null)
                {
                    matchcolpart = i;

                    /*matches++;

                    if (matches > 1)
                    {
                        throw NameResolutionError.AmbigousColumnReference(ncr);
                    }*/

                    return ncr;
                }
            }

            return null;
        }

        private ColumnReference ResolveMemberAccessListAsColumn(int colpart)
        {
            ColumnReference ncr;
            var tokens = visitor.CurrentMemberAccessList;
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

                SubstituteSourceTableDefaults(sourceTableCollection, tr, false);
                tr = ResolveSourceTableReference(sourceTableCollection, tr, false);

                if (tr == null)
                {
                    return null;
                }
                else
                {
                    cr.TableReference = tr;
                    ncr = ResolveColumnReference(cr);
                }
            }

            return ncr;
        }

        private FunctionReference ResolveMemberAccessListAsFunction(out int matchcolpart)
        {
            var tokens = visitor.CurrentMemberAccessList;

            // Find the very first MemberCall token which could be a function call
            // instead of a method call
            matchcolpart = -1;
            for (int i = 1; i < tokens.Count; i++)
            {
                if (tokens[i] is MemberCall)
                {
                    matchcolpart = i;
                    break;
                }
            }

            // Function names can at most be in the form of schema.function
            // or database.schema.function
            if (matchcolpart < 1 || matchcolpart > 2)
            {
                return null;
            }

            var fr = FunctionReference.Interpret(tokens, matchcolpart);
            SubstituteFunctionDefaults(fr);
            fr = ResolveFunctionReference(fr);

            return fr;
        }

        private void ResolveMemberAccessList(Operand operand, int matchcolpart)
        {
            var tokens = visitor.CurrentMemberAccessList;

            // Iterate through the rest of the tokens and create properties / method calls
            if (matchcolpart + 1 < tokens.Count)
            {
                var nma = new Node[tokens.Count - matchcolpart - 1];

                // Everything else after the column name is properties or methods
                for (int i = 0; i < nma.Length; i++)
                {
                    var token = tokens[i + matchcolpart + 1];

                    if (token is MemberAccess)
                    {
                        var pr = new PropertyReference()
                        {
                            PropertyName = ReferenceBase.RemoveIdentifierQuotes(token.Value)
                        };
                        nma[i] = UdtPropertyAccess.Create(pr);
                    }
                    else if (token is MemberCall)
                    {
                        var args = ((MemberCall)token).GetArguments();
                        var mr = new MethodReference()
                        {
                            MethodName = ReferenceBase.RemoveIdentifierQuotes(((MemberCall)token).MemberName.Value)
                        };
                        nma[i] = UdtMethodCall.Create(mr, args);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                var mal = MemberAccessList.Create(nma);
                var oml = operand.MemberAccessList;

                if (mal != null)
                {
                    if (oml != null)
                    {
                        oml.ReplaceWith(mal);
                    }
                    else
                    {
                        operand.Stack.AddLast(mal);
                    }
                }
                else if (oml != null)
                {
                    oml.Remove();
                }
            }
            else
            {
                var oml = operand.MemberAccessList;

                if (oml != null)
                {
                    oml.Remove();
                }
            }
        }

        #endregion
        #region Default substitution logic

        /// <summary>
        /// When overriden, substitutes table defaults for all tables that are supposed to exist during query execution
        /// </summary>
        /// <param name="resolvedSourceTables"></param>
        /// <param name="tr"></param>
        public void SubstituteSourceTableDefaults(ISourceTableProvider resolvedSourceTables, TableReference tr, bool excludeOuterQueries)
        {
            try
            {
                if (tr.IsPossiblyAlias && FindTableByAlias(tr, excludeOuterQueries) != null)
                {
                    // Don't do any substitution if referencing a common table or anything that aliased
                }
                else if (tr.TableContext.HasFlag(TableContext.UserDefinedFunction))
                {
                    OnSubstituteTableDefaults(tr);
                }
                else if (tr.TableContext.HasFlag(TableContext.Variable))
                {
                    // No substitution
                }
                else
                {
                    OnSubstituteTableDefaults(tr);
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw NameResolutionError.UnresolvableDatasetReference(ex, tr);
            }
        }

        protected virtual void OnSubstituteTableDefaults(TableReference tr)
        {
        }

        protected virtual void OnSubstituteFunctionDefaults(TableReference tr)
        {
        }

        /// <summary>
        /// When overriden, substitutes table default for tables that are created during query execution 
        /// </summary>
        /// <param name="tr"></param>
        public void SubstituteOutputTableDefaults(TableReference tr)
        {
            OnSubstituteOutputTableDefaults(tr);
        }

        protected virtual void OnSubstituteOutputTableDefaults(TableReference tr)
        {
            SubstituteDefaults(tr);
        }

        /// <summary>
        /// When overriden, substitutes dataset and schema defaults into function references.
        /// </summary>
        /// /// <remarks>
        /// This is non-standard SQL as SQL requires the schema name to be specified and the database is
        /// always taken from the current context. In applications, like SkyQuery, functions are always
        /// taken from the CODE database.
        /// </remarks>
        /// <param name="node"></param>
        public void SubstituteFunctionDefaults(FunctionReference fr)
        {
            if (String.IsNullOrWhiteSpace(fr.SchemaName) && IsSystemFunctionName(fr.FunctionName))
            {
                fr.IsSystem = true;
            }
            else
            {
                OnSubstituteFunctionDefaults(fr);
            }
        }

        protected virtual void OnSubstituteFunctionDefaults(FunctionReference fr)
        {
        }

        public void SubstituteDataTypeDefaults(DataTypeReference dr)
        {
            if (!dr.IsSystem)
            {
                OnSubstituteDataTypeDefaults(dr);
            }
        }

        protected virtual void OnSubstituteDataTypeDefaults(DataTypeReference dr)
        {
        }

        /// <summary>
        /// Substitute default dataset and schema names, if necessary
        /// </summary>
        /// <param name="defaultDataSetName"></param>
        /// <param name="defaultSchemaName"></param>
        private void SubstituteDefaults(DatabaseObjectReference dr)
        {
            // This cannot be called for subqueries

            if (dr.DatasetName == null)

                if (dr.DatabaseName == null)
                {
                    dr.DatabaseName = dataset.DatabaseName;
                }

            if (dr.SchemaName == null)
            {
                dr.SchemaName = dataset.DefaultSchemaName;
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
            if (tr.TableContext.HasFlag(TableContext.TableOrView))
            {
                var uniqueKey = tr.DatabaseObject.UniqueKey;

                if (!details.TargetTableReferences.ContainsKey(uniqueKey))
                {
                    details.TargetTableReferences.Add(uniqueKey, new List<TableReference>());
                }

                details.TargetTableReferences[uniqueKey].Add(tr);
            }
        }

        private void CollectOutputTableReference(ITargetTableProvider targetTable, TableReference tr)
        {
            var table = (Table)tr.DatabaseObject;
            var uniqueKey = table.UniqueKey;

            // Add to schema, but do not enforce uniqueness because conditional
            // execution might produces tables with the same name through
            // different branches
            table.Dataset.Tables.TryAdd(uniqueKey, table);

            // Add to query details
            if (!details.OutputTableReferences.ContainsKey(uniqueKey))
            {
                details.OutputTableReferences.Add(uniqueKey, new List<TableReference>());
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

            // Generate a default column key if necessary
            int q = 0;
            var key = ncr.ColumnName;
            while (String.IsNullOrWhiteSpace(key) || tr.ColumnReferences.ContainsKey(key))
            {
                key = String.Format("__Col_{0}", q);
                q++;
            }

            tr.ColumnReferences.Add(key, ncr);
            return ncr;
        }

        #endregion
    }
}
