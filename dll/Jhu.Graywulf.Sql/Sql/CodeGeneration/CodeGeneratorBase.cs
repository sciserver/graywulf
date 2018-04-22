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

namespace Jhu.Graywulf.Sql.CodeGeneration
{
    public abstract class CodeGeneratorBase : CodeGenerator
    {
        #region Private members

        private IdentifierQuoting identifierQuoting;

        private NameRendering tableNameRendering;
        private AliasRendering tableAliasRendering;
        private NameRendering columnNameRendering;
        private AliasRendering columnAliasRendering;
        private NameRendering functionNameRendering;

        private Lazy<Dictionary<DatasetBase, DatasetBase>> datasetMap;
        private Lazy<Dictionary<TableReference, TableReference>> tableReferenceMap;
        private Lazy<Dictionary<ColumnReference, ColumnReference>> columnReferenceMap;
        private Lazy<Dictionary<DataTypeReference, DataTypeReference>> dataTypeReferenceMap;
        private Lazy<Dictionary<FunctionReference, FunctionReference>> functionReferenceMap;
        private Lazy<Dictionary<VariableReference, VariableReference>> variableReferenceMap;

        #endregion
        #region Properties

        public IdentifierQuoting IdentifierQuoting
        {
            get { return identifierQuoting; }
            set { identifierQuoting = value; }
        }

        /// <summary>
        /// Gets or sets whether to use resolved names in the generated code
        /// </summary>
        public NameRendering TableNameRendering
        {
            get { return tableNameRendering; }
            set { tableNameRendering = value; }
        }

        public AliasRendering TableAliasRendering
        {
            get { return tableAliasRendering; }
            set { tableAliasRendering = value; }
        }

        public NameRendering ColumnNameRendering
        {
            get { return columnNameRendering; }
            set { columnNameRendering = value; }
        }

        public AliasRendering ColumnAliasRendering
        {
            get { return columnAliasRendering; }
            set { columnAliasRendering = value; }
        }

        public NameRendering FunctionNameRendering
        {
            get { return functionNameRendering; }
            set { functionNameRendering = value; }
        }

        public Dictionary<DatasetBase, DatasetBase> DatasetMap
        {
            get { return datasetMap.Value; }
        }

        public Dictionary<TableReference, TableReference> TableReferenceMap
        {
            get { return tableReferenceMap.Value; }
        }

        public Dictionary<ColumnReference, ColumnReference> ColumnReferenceMap
        {
            get { return columnReferenceMap.Value; }
        }

        public Dictionary<DataTypeReference, DataTypeReference> DataTypeReferenceMap
        {
            get { return dataTypeReferenceMap.Value; }
        }

        public Dictionary<FunctionReference, FunctionReference> FunctionReferenceMap
        {
            get { return functionReferenceMap.Value; }
        }

        public Dictionary<VariableReference, VariableReference> VariableReferenceMap
        {
            get { return variableReferenceMap.Value; }
        }

        #endregion
        #region Constructors and initializers

        protected CodeGeneratorBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.identifierQuoting = IdentifierQuoting.AlwaysQuote;

            this.tableNameRendering = NameRendering.Default;
            this.tableAliasRendering = AliasRendering.Default;
            this.columnNameRendering = NameRendering.Default;
            this.columnAliasRendering = AliasRendering.Default;
            this.functionNameRendering = NameRendering.Default;

            // TODO: how to compare datasets?
            this.datasetMap = new Lazy<Dictionary<DatasetBase, DatasetBase>>(() => new Dictionary<DatasetBase, DatasetBase>());
            this.tableReferenceMap = new Lazy<Dictionary<TableReference, TableReference>>(() => new Dictionary<TableReference, TableReference>(TableReferenceEqualityComparer.Default));
            this.columnReferenceMap = new Lazy<Dictionary<ColumnReference, ColumnReference>>(() => new Dictionary<ColumnReference, ColumnReference>(ColumnReferenceEqualityComparer.Default));
            this.dataTypeReferenceMap = new Lazy<Dictionary<DataTypeReference, DataTypeReference>>(() => new Dictionary<DataTypeReference, DataTypeReference>(DataTypeReferenceEqualityComparer.Default));
            this.functionReferenceMap = new Lazy<Dictionary<FunctionReference, FunctionReference>>(() => new Dictionary<FunctionReference, FunctionReference>(FunctionReferenceEqualityComparer.Default));
            this.variableReferenceMap = new Lazy<Dictionary<VariableReference, VariableReference>>(() => new Dictionary<VariableReference, VariableReference>(VariableReferenceEqualityComparer.Default));
        }

        #endregion

        public abstract ColumnListGeneratorBase CreateColumnListGenerator();

        public ColumnListGeneratorBase CreateColumnListGenerator(TableReference table, ColumnContext columnContext, ColumnListType listType)
        {
            var cg = CreateColumnListGenerator();
            cg.ListType = listType;
            cg.Columns.AddRange(table.FilterColumnReferences(columnContext));
            return cg;
        }

        #region Identifier formatting functions

        /// <summary>
        /// Puts quotes around an identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <remarks>
        /// The quoting characters used depends on the flavor of SQL
        /// </remarks>
        protected abstract string GetQuotedIdentifier(string identifier);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <remarks>
        /// Unquoting depends on the actual input and not on the
        /// code generator, so this function is implemented as non-
        /// virtual
        /// </remarks>
        public string UnquoteIdentifier(string identifier)
        {
            if (identifier[0] == '[' && identifier[identifier.Length - 1] == ']')
            {
                return identifier.Substring(1, identifier.Length - 2);
            }
            else
            {
                return identifier;
            }
        }

        public string EscapeIdentifier(string identifier)
        {
            return identifier.Replace(".", "_");
        }

        protected T MapDataset<T>(T databaseObjectReference)
            where T : DatabaseObjectReference
        {
            var ds = databaseObjectReference?.DatabaseObject?.Dataset;

            if (ds != null && datasetMap.IsValueCreated && datasetMap.Value.ContainsKey(ds))
            {
                var ndo = (T)databaseObjectReference.Clone();
                ndo.DatabaseObject = (DatabaseObject)ndo.DatabaseObject.Clone();
                ndo.DatabaseObject.Dataset = datasetMap.Value[ds];
                return ndo;
            }
            else
            {
                return databaseObjectReference;
            }
        }

        protected TableReference MapTableReference(TableReference table)
        {
            if (tableReferenceMap.IsValueCreated && tableReferenceMap.Value.ContainsKey(table))
            {
                table = tableReferenceMap.Value[table];
            }

            return table;
        }

        public string GenerateEscapedUniqueName(TableReference table)
        {
            if (table.Type == TableReferenceType.Subquery ||
                table.Type == TableReferenceType.CommonTable ||
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

        public string GetResolvedTableName(TableReference table)
        {
            table = MapTableReference(table);

            if (table.Type == TableReferenceType.Subquery ||
                table.Type == TableReferenceType.CommonTable ||
                table.IsComputed)
            {
                return GetQuotedIdentifier(table.Alias);
            }
            else
            {
                if (table.DatabaseObject != null)
                {
                    // If it is linked up to the schema already
                    return GetResolvedTableName(table.DatabaseObject);
                }
                else
                {
                    // If it's not resolved yet against the schema
                    return GetResolvedTableName(table.DatabaseName, table.SchemaName, table.DatabaseObjectName);
                }
            }
        }

        public string GetResolvedTableNameWithAlias(TableReference table)
        {
            table = MapTableReference(table);

            if (table.Type == TableReferenceType.Subquery ||
                table.Type == TableReferenceType.CommonTable ||
                table.IsComputed)
            {
                return GetQuotedIdentifier(table.Alias);
            }
            else
            {
                string name;

                if (table.DatabaseObject != null)
                {
                    // If it is linked up to the schema already
                    name = GetResolvedTableName(table.DatabaseObject);
                }
                else
                {
                    // If it's not resolved yet against the schema
                    name = GetResolvedTableName(table.DatabaseName, table.SchemaName, table.DatabaseObjectName);
                }

                if (String.IsNullOrEmpty(table.Alias))
                {
                    return name;
                }
                else
                {
                    return name + " AS " + GetQuotedIdentifier(table.Alias);
                }
            }
        }

        public string GetUniqueName(TableReference table)
        {
            table = MapTableReference(table);

            if (table.Type == TableReferenceType.Subquery ||
                table.Type == TableReferenceType.CommonTable ||
                table.IsComputed ||
                !String.IsNullOrWhiteSpace(table.Alias))
            {
                return GetQuotedIdentifier(table.Alias);
            }
            else
            {
                return GetResolvedTableName(table);
            }
        }

        public string GeneratePrimaryKeyName(DatabaseObject table)
        {
            return GeneratePrimaryKeyName(table.SchemaName, table.ObjectName);
        }

        public string GeneratePrimaryKeyName(string schemaName, string tableName)
        {
            return GetQuotedIdentifier(String.Format("PK_{0}_{1}", schemaName, tableName));
        }

        public string GetResolvedTableName(DatabaseObject table)
        {
            return GetResolvedTableName(table.DatabaseName, table.SchemaName, table.ObjectName);
        }

        public string GetResolvedTableNameWithAlias(DatabaseObject table, string alias)
        {
            return GetResolvedTableNameWithAlias(table.DatabaseName, table.SchemaName, table.ObjectName, alias);
        }

        protected abstract string GetResolvedTableName(string databaseName, string schemaName, string tableName);

        protected virtual string GetResolvedTableNameWithAlias(string databaseName, string schemaName, string tableName, string alias)
        {
            if (String.IsNullOrWhiteSpace(alias))
            {
                return GetResolvedTableName(databaseName, schemaName, tableName);
            }
            else
            {
                return GetResolvedTableName(databaseName, schemaName, tableName) + " AS " + GetQuotedIdentifier(alias);
            }
        }

        protected ColumnReference MapColumnReference(ColumnReference column)
        {
            if (columnReferenceMap.IsValueCreated && columnReferenceMap.Value.ContainsKey(column))
            {
                return columnReferenceMap.Value[column];
            }
            else
            {
                return column;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks>
        /// In this function call, tables are always referenced by
        /// their aliases, if they have one.
        /// </remarks>
        public string GetResolvedColumnName(ColumnReference column)
        {
            column = MapColumnReference(column);
            var table = MapTableReference(column.TableReference);

            string tablename;

            if (table == null)
            {
                tablename = null;
            }
            else if (!String.IsNullOrEmpty(table.Alias))
            {
                tablename = GetQuotedIdentifier(table.Alias);
            }
            else
            {
                tablename = GetResolvedTableName(table);
            }

            string columnname;

            if (column.IsStar)
            {
                columnname = "*";
            }
            else
            {
                columnname = GetQuotedIdentifier(column.ColumnName);
            }


            string res = String.Empty;

            if (!String.IsNullOrWhiteSpace(tablename))
            {
                res = tablename + ".";
            }

            res += columnname;

            return res;
        }

        protected FunctionReference MapFunctionReference(FunctionReference function)
        {
            if (functionReferenceMap.IsValueCreated && functionReferenceMap.Value.ContainsKey(function))
            {
                return functionReferenceMap.Value[function];
            }
            else
            {
                return function;
            }
        }

        private string GetResolvedFunctionName(FunctionReference function)
        {
            function = MapFunctionReference(function);

            if (function.IsSystem)
            {
                // This is a built-in function
                return function.SystemFunctionName.ToUpperInvariant();
            }
            else if (function.DatabaseObject != null)
            {
                // If it is linked up to the schema already
                return GetResolvedFunctionName(function.DatabaseObject.DatabaseName, function.DatabaseObject.SchemaName, function.DatabaseObject.ObjectName);
            }
            else
            {
                // If it's not resolved yet against the schema
                return GetResolvedFunctionName(function.DatabaseName, function.SchemaName, function.DatabaseObjectName);
            }
        }

        protected abstract string GetResolvedFunctionName(string databaseName, string schemaName, string functionName);

        #endregion
        #region Specialized node writer functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <remarks>
        /// Do dispatching here based on node type
        /// </remarks>
        protected override void WriteNode(Token token)
        {
            switch (token)
            {
                case TableAlias ta:
                    WriteTableAlias(ta);
                    break;
                case TableOrViewName t:
                    WriteTableOrViewName(t);
                    break;
                case ColumnIdentifier ci:
                    WriteColumnIdentifier(ci);
                    break;
                case FunctionIdentifier fi:
                    WriteFunctionIdentifier(fi);
                    break;
                /* TODO: implement these
                case UserVariable v:
                    WriteUserVariable(v);
                    break;
                case Parsing.DataType dt:
                    WriteDataType(dt);
                    break;
                */
                case ColumnExpression ce:
                    WriteColumnExpression(ce);
                    break;
                default:
                    base.WriteNode(token);
                    break;
            }
        }

        public void WriteTableAlias(TableAlias node)
        {
            switch (tableAliasRendering)
            {
                case AliasRendering.Default:
                case AliasRendering.Always:
                    Writer.Write(GetQuotedIdentifier(node.Value));
                    break;
                case AliasRendering.Never:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>
        /// Table names are only written by this function when the
        /// table appears in the FROM clause. In all other cases it's
        /// WriteColumnIdentifier that generates the output
        /// </remarks>
        public void WriteTableOrViewName(TableOrViewName node)
        {
            switch (tableNameRendering)
            {
                case NameRendering.FullyQualified:
                    Writer.Write(GetResolvedTableName(node.TableReference));
                    break;
                case NameRendering.IdentifierOnly:
                    Writer.Write(GetQuotedIdentifier(node.TableReference.DatabaseObjectName));
                    break;
                default:
                    base.WriteNode(node);
                    break;
            }
        }

        /// <summary>
        /// Writes a column identifier, optionally with resolved
        /// names and quoted
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public void WriteColumnIdentifier(ColumnIdentifier node)
        {
            switch (columnNameRendering)
            {
                case NameRendering.FullyQualified:
                    Writer.Write(GetResolvedColumnName(node.ColumnReference));
                    break;
                case NameRendering.IdentifierOnly:
                    Writer.Write(GetQuotedIdentifier(node.ColumnReference.ColumnName));
                    break;
                default:
                    base.WriteNode(node);
                    break;
            }
        }

        /// <summary>
        /// Writes a column expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual void WriteColumnExpression(ColumnExpression node)
        {
            // A column expression is in the form of an expression,
            // optionally followed by a column alias in the form of
            // 'AS alias'

            // TODO: test with @v = column AND alias = column syntax

            var variable = node.AssignedVariable;

            if (variable == null)
            {
                if (columnAliasRendering == AliasRendering.Always)
                {
                    // Write the expression first as it is
                    var exp = node.FindDescendant<Parsing.Expression>();
                    WriteNode(exp);

                    // If it's not a * column and there's an alias, write it
                    if (!node.ColumnReference.IsStar && !String.IsNullOrEmpty(node.ColumnReference.ColumnAlias))
                    {
                        Writer.Write(
                            " AS {0}",
                            GetQuotedIdentifier(node.ColumnReference.ColumnAlias));
                    }
                }
                else if (columnAliasRendering == AliasRendering.Never)
                {
                    var exp = node.FindDescendant<Parsing.Expression>();
                    WriteNode(exp);
                }
                else
                {
                    // Fall back to original behavior
                    base.WriteNode(node);
                }
            }
            else
            {
                // Fall back to original behavior
                base.WriteNode(node);
            }
        }

        public virtual void WriteFunctionIdentifier(FunctionIdentifier node)
        {
            switch (functionNameRendering)
            {
                case NameRendering.FullyQualified:
                    Writer.Write(GetResolvedFunctionName(node.FunctionReference));
                    break;
                case NameRendering.IdentifierOnly:
                    // No point doing this because it would break the query
                    throw new InvalidOperationException();
                default:
                    base.WriteNode(node);
                    break;
            }
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
