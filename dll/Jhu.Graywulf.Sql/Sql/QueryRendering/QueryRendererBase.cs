using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.QueryRendering
{
    public abstract class QueryRendererBase
    {
        #region Private members

        private TextWriter writer;

        private QueryRendererOptions options;

        private Lazy<Dictionary<DatasetBase, DatasetBase>> datasetMap;
        private Lazy<Dictionary<TableReference, TableReference>> tableReferenceMap;
        private Lazy<Dictionary<ColumnReference, ColumnReference>> columnReferenceMap;
        private Lazy<Dictionary<DataTypeReference, DataTypeReference>> dataTypeReferenceMap;
        private Lazy<Dictionary<FunctionReference, FunctionReference>> functionReferenceMap;
        private Lazy<Dictionary<VariableReference, VariableReference>> variableReferenceMap;

        #endregion
        #region Properties

        protected TextWriter Writer
        {
            get { return writer; }
        }

        public QueryRendererOptions Options
        {
            get { return options; }
            set { options = value; }
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

        protected QueryRendererBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.writer = null;
            this.options = CreateOptions();
            // TODO: how to compare datasets?
            this.datasetMap = new Lazy<Dictionary<DatasetBase, DatasetBase>>(() => new Dictionary<DatasetBase, DatasetBase>());
            this.tableReferenceMap = new Lazy<Dictionary<TableReference, TableReference>>(() => new Dictionary<TableReference, TableReference>(TableReferenceEqualityComparer.Default));
            this.columnReferenceMap = new Lazy<Dictionary<ColumnReference, ColumnReference>>(() => new Dictionary<ColumnReference, ColumnReference>(ColumnReferenceEqualityComparer.Default));
            this.dataTypeReferenceMap = new Lazy<Dictionary<DataTypeReference, DataTypeReference>>(() => new Dictionary<DataTypeReference, DataTypeReference>(DataTypeReferenceEqualityComparer.Default));
            this.functionReferenceMap = new Lazy<Dictionary<FunctionReference, FunctionReference>>(() => new Dictionary<FunctionReference, FunctionReference>(FunctionReferenceEqualityComparer.Default));
            this.variableReferenceMap = new Lazy<Dictionary<VariableReference, VariableReference>>(() => new Dictionary<VariableReference, VariableReference>(VariableReferenceEqualityComparer.Default));
        }

        protected virtual QueryRendererOptions CreateOptions()
        {
            return new QueryRendererOptions();
        }

        #endregion
        #region Public driver functions

        public virtual string Execute(Node node)
        {
            if (node == null)
            {
                return String.Empty;
            }
            else
            {
                using (writer = new StringWriter())
                {
                    TraverseTopDown(node);
                    return writer.ToString();
                }
            }
        }

        public virtual void Execute(TextWriter writer, Node node)
        {
            this.writer = writer;
            TraverseTopDown(node);
        }

        #endregion
        #region Node traversal

        protected void TraverseTopDown(Node node)
        {
            var res = WriteNode((dynamic)node);

            if (!res)
            {
                foreach (var n in node.Stack)
                {
                    switch (n)
                    {
                        case Node nn:
                            TraverseTopDown(nn);
                            break;
                        case Token t:
                            WriteToken(t);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        protected bool WriteNode(Node node)
        {
            return false;
        }

        protected virtual void WriteToken(Token token)
        {
            writer.Write(token.Value);
        }

        #endregion
        #region Specialized node visitors

        protected virtual bool WriteNode(MagicTokenBase node)
        {
            node.Write(this, Writer);
            return true;
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
        protected virtual bool WriteNode(TableOrViewIdentifier node)
        {
            switch (options.TableNameRendering)
            {
                case NameRendering.FullyQualified:
                    Writer.Write(GetResolvedTableName(node.TableReference));
                    return true;
                case NameRendering.IdentifierOnly:
                    Writer.Write(GetQuotedIdentifier(node.TableReference.DatabaseObjectName));
                    return true;
                default:
                    return false;
            }
        }

        protected virtual bool WriteNode(TableAlias node)
        {
            switch (options.TableAliasRendering)
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

            return true;
        }

        protected virtual bool WriteNode(TargetTableSpecification tts)
        {
            if ((tts.TableReference.TableContext & TableContext.Target) != 0 &&
                !String.IsNullOrEmpty(tts.TableReference.Alias))
            {
                // Always render target tables with alias, when defined
                Writer.Write(GetQuotedIdentifier(tts.TableReference.Alias));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Writes a column identifier, optionally with resolved
        /// names and quoted
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected virtual bool WriteNode(ColumnIdentifier node)
        {
            switch (options.ColumnNameRendering)
            {
                case NameRendering.FullyQualified:
                    Writer.Write(GetResolvedColumnName(node.ColumnReference));
                    return true;
                case NameRendering.IdentifierOnly:
                    Writer.Write(GetQuotedIdentifier(node.ColumnReference.ColumnName));
                    return true;
                default:
                    return false;
            }
        }

        protected virtual bool WriteNode(StarColumnIdentifier node)
        {
            var tr = node.TableOrViewIdentifier?.TableReference;

            if (tr != null)
            {
                if (!String.IsNullOrWhiteSpace(tr.Alias))
                {
                    switch (options.TableNameRendering)
                    {
                        case NameRendering.FullyQualified:
                        case NameRendering.IdentifierOnly:
                            Writer.Write(GetQuotedIdentifier(tr.Alias));
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (options.TableNameRendering)
                    {
                        case NameRendering.FullyQualified:
                            Writer.Write(GetResolvedTableName(tr));
                            break;
                        case NameRendering.IdentifierOnly:
                            Writer.Write(GetQuotedIdentifier(tr.TableName));
                            break;
                        default:
                            break;
                    }
                }

                Writer.Write(".");
            }

            Writer.Write("*");

            return true;
        }

        /// <summary>
        /// Writes a column expression
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected virtual bool WriteNode(ColumnExpression node)
        {
            // A column expression is in the form of an expression,
            // optionally followed by a column alias in the form of
            // 'AS alias'

            // TODO: test with @v = column AND alias = column syntax

            var variable = node.AssignedVariable;
            var exp = node.Expression;
            var star = node.StarColumnIdentifier;

            if (options.ColumnAliasRendering == AliasRendering.Default)
            {
                return false;
            }
            else
            {
                if (star != null)
                {
                    TraverseTopDown(star);
                }
                else if (variable != null)
                {
                    TraverseTopDown(variable);
                }
                else if (exp != null)
                {
                    TraverseTopDown(exp);
                }
                else
                {
                    throw new NotImplementedException();
                }

                if (options.ColumnAliasRendering == AliasRendering.Always)
                {
                    if (!node.ColumnReference.IsStar && !String.IsNullOrEmpty(node.ColumnReference.ColumnAlias))
                    {
                        Writer.Write(" AS {0}", GetQuotedIdentifier(node.ColumnReference.ColumnAlias));
                    }

                    return true;
                }
                else if (options.ColumnAliasRendering == AliasRendering.Never)
                {
                    return true;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        protected virtual bool WriteNode(ColumnName node)
        {
            // Table and index column definition
            // UPDATE set mutator only

            switch (options.ColumnNameRendering)
            {
                case NameRendering.FullyQualified:
                case NameRendering.IdentifierOnly:
                    Writer.Write(GetQuotedIdentifier(node.ColumnReference.ColumnName));
                    return true;
                default:
                    return false;
            }
        }

        protected virtual bool WriteNode(PropertyName node)
        {
            switch (options.UdtMemberNameRendering)
            {
                case NameRendering.FullyQualified:
                case NameRendering.IdentifierOnly:
                    Writer.Write(GetQuotedIdentifier(node.PropertyReference.PropertyName));
                    return true;
                default:
                    return false;
            }
        }

        protected virtual bool WriteNode(MethodName node)
        {
            switch (options.UdtMemberNameRendering)
            {
                case NameRendering.FullyQualified:
                case NameRendering.IdentifierOnly:
                    Writer.Write(GetQuotedIdentifier(node.MethodReference.MethodName));
                    return true;
                default:
                    return false;
            }
        }

        protected virtual bool WriteNode(DataTypeSpecification node)
        {
            WriteDataTypeName(node);
            return true;
        }

        protected virtual bool WriteNode(DataTypeIdentifier node)
        {
            return WriteDataTypeName(node);
        }

        protected virtual bool WriteNode(FunctionIdentifier node)
        {
            return WriteFunctionName(node);
        }

        protected virtual bool WriteNode(UserVariable node)
        {
            return WriteVariable(node);
        }

        protected virtual bool WriteNode(SystemVariable node)
        {
            return WriteVariable(node);
        }

        protected virtual bool WriteNode(IndexName node)
        {
            WriteIndexName(node);
            return true;
        }

        protected virtual bool WriteNode(ConstraintName node)
        {
            WriteConstraintName(node);
            return true;
        }

        #endregion
        #region Specialized writer functions

        private bool WriteVariable(IVariableReference node)
        {
            switch (options.VariableRendering)
            {
                case VariableRendering.Substitute:
                    Writer.Write(GetResolvedVariableName(node.VariableReference));
                    return true;
                default:
                    return false;
            }
        }
        
        private bool WriteFunctionName(IFunctionReference node)
        {
            switch (options.FunctionNameRendering)
            {
                case NameRendering.FullyQualified:
                    Writer.Write(GetResolvedFunctionName(node.FunctionReference));
                    return true;
                case NameRendering.IdentifierOnly:
                    // No point doing this because it would break the query
                    throw new InvalidOperationException();
                default:
                    return false;
            }
        }

        private bool WriteDataTypeName(IDataTypeReference node)
        {
            switch (options.DataTypeNameRendering)
            {
                case NameRendering.FullyQualified:
                    Writer.Write(GetResolvedDataTypeName(node.DataTypeReference));
                    return true;
                case NameRendering.IdentifierOnly:
                    // No point doing this because it would break the query
                    throw new InvalidOperationException();
                default:
                    return false;
            }
        }

        private bool WriteIndexName(IIndexReference node)
        {
            switch (options.IndexNameRendering)
            {
                case NameRendering.FullyQualified:
                case NameRendering.IdentifierOnly:
                    Writer.Write(GetQuotedIdentifier(node.IndexReference.IndexName));
                    return true;
                default:
                    return false;
            }
        }

        private bool WriteConstraintName(IConstraintReference node)
        {
            switch (options.ConstraintNameRendering)
            {
                case NameRendering.FullyQualified:
                case NameRendering.IdentifierOnly:
                    Writer.Write(GetQuotedIdentifier(node.ConstraintReference.ConstraintName));
                    return true;
                default:
                    return false;
            }
        }

        #endregion
        #region Identifier formatting functions

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

        /// <summary>
        /// Puts quotes around an identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <remarks>
        /// The quoting characters used depends on the flavor of SQL
        /// </remarks>
        public abstract string GetQuotedIdentifier(string identifier);

        #endregion
        #region Reference mapping

        public T MapDataset<T>(T databaseObjectReference)
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

        public TableReference MapTableReference(TableReference table)
        {
            if (tableReferenceMap.IsValueCreated && tableReferenceMap.Value.ContainsKey(table))
            {
                table = tableReferenceMap.Value[table];
            }

            return table;
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

        protected DataTypeReference MapDataTypeReference(DataTypeReference dataType)
        {
            if (dataTypeReferenceMap.IsValueCreated && dataTypeReferenceMap.Value.ContainsKey(dataType))
            {
                return dataTypeReferenceMap.Value[dataType];
            }
            else
            {
                return dataType;
            }
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

        protected VariableReference MapVariableReference(VariableReference variable)
        {
            if (variableReferenceMap.IsValueCreated && variableReferenceMap.Value.ContainsKey(variable))
            {
                return variableReferenceMap.Value[variable];
            }
            else
            {
                return variable;
            }
        }

        #endregion
        #region Resolved table names

        public string GetResolvedTableName(TableReference table)
        {
            table = MapTableReference(table);

            if (table.TableContext.HasFlag(TableContext.Subquery) ||
                table.TableContext.HasFlag(TableContext.CommonTable) ||
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

            if (table.TableContext.HasFlag(TableContext.Subquery) ||
                table.TableContext.HasFlag(TableContext.CommonTable) ||
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

            if (table.TableContext.HasFlag(TableContext.Subquery) ||
                table.TableContext.HasFlag(TableContext.CommonTable) ||
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

        public abstract string GetResolvedTableName(string databaseName, string schemaName, string tableName);

        public virtual string GetResolvedTableNameWithAlias(string databaseName, string schemaName, string tableName, string alias)
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

        #endregion
        #region Resolved column names

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
            else if (!String.IsNullOrEmpty(table.VariableName))
            {
                tablename = GetQuotedIdentifier(table.VariableName);
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

        #endregion
        #region Resolved data type names

        public string GetResolvedDataTypeName(DataTypeReference dataType)
        {
            dataType = MapDataTypeReference(dataType);

            if (dataType.IsSystem)
            {
                // This is a built-in function
                return GetResolvedDataTypeName(dataType.DataType);
            }
            else if (dataType.DatabaseObject != null)
            {
                // If it is linked up to the schema already
                return GetResolvedDataTypeName(dataType.DatabaseObject.DatabaseName, dataType.DatabaseObject.SchemaName, dataType.DatabaseObject.ObjectName);
            }
            else
            {
                // If it's not resolved yet against the schema
                return GetResolvedDataTypeName(dataType.DatabaseName, dataType.SchemaName, dataType.DatabaseObjectName);
            }
        }

        public abstract string GetResolvedDataTypeName(DataType dataType);

        public abstract string GetResolvedDataTypeName(string databaseName, string schemaName, string functionName);

        #endregion
        #region Resolved function names

        public string GetResolvedFunctionName(FunctionReference function)
        {
            function = MapFunctionReference(function);

            if (function.IsSystem)
            {
                // This is a built-in function
                return function.FunctionName.ToUpperInvariant();
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

        public abstract string GetResolvedFunctionName(string databaseName, string schemaName, string functionName);

        #endregion
        #region Resolved variable names

        public string GetResolvedVariableName(VariableReference variable)
        {
            variable = MapVariableReference(variable);
            return variable.VariableName;
        }

        #endregion
    }
}
