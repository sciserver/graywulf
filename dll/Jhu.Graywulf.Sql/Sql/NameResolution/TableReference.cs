using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class TableReference : DatabaseObjectReference
    {
        #region Property storage variables

        private string alias;
        private TableContext tableContext;
        private bool isComputed;
        private bool isResolved;

        private List<ColumnReference> columnReferences;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the resolved alias
        /// </summary>
        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }

        public TableContext TableContext
        {
            get { return tableContext; }
            set { tableContext = value; }
        }

        public TableOrView TableOrView
        {
            get { return (TableOrView)DatabaseObject; }
        }

        /// <summary>
        /// Gets a value indicating whether the table source is computed by custom code.
        /// </summary>
        /// <remarks>
        /// This is an extension to traditional SQL queries to support tables that are
        /// calculated during multi-step execution, for instance the xmatch results table
        /// in sky-query.
        /// </remarks>
        public bool IsComputed
        {
            get { return isComputed; }
            set { isComputed = value; }
        }

        public bool IsResolved
        {
            get { return isResolved; }
            set { isResolved = value; }
        }
        
        public bool IsCachable
        {
            get
            {
                return
                  !tableContext.HasFlag(TableContext.Subquery) &&
                  !tableContext.HasFlag(TableContext.CommonTable) &&
                  !tableContext.HasFlag(TableContext.UserDefinedFunction) &&
                  !tableContext.HasFlag(TableContext.CreateTable) &&
                  !tableContext.HasFlag(TableContext.Target) &&             // TODO: review this
                  !IsComputed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// A table reference might be an alias if only the table name part is specified.
        /// </remarks>
        public bool IsPossiblyAlias
        {
            get
            {
                return (alias != null || DatabaseObjectName != null) && DatasetName == null && DatabaseName == null;
            }
        }

        public override bool IsUndefined
        {
            get { return base.IsUndefined && alias == null; }
        }

        /// <summary>
        /// Gets the unique name of the table (alias, if available)
        /// </summary>
        /// <remarks>
        /// Never use this in query generation!
        /// </remarks>
        public override string UniqueName
        {
            get
            {
                // TODO: review this and make sure kez is unique even if table
                // is referenced deep down in CTEs

                if (String.IsNullOrWhiteSpace(alias))
                {
                    return base.UniqueName;
                }
                else
                {
                    return String.Format("[{0}]", alias);
                }
            }
        }

        /// <summary>
        /// Returns the exported name of a subquery or a table
        /// </summary>
        public string ExportedName
        {
            get
            {
                if (tableContext.HasFlag(TableContext.Subquery) ||
                    tableContext.HasFlag(TableContext.CommonTable) ||
                    tableContext.HasFlag(TableContext.UserDefinedFunction) ||
                    isComputed ||
                    alias != null)
                {
                    return alias;
                }
                else
                {
                    // If no alias is used then use table name
                    // SQL Server doesn't allow two tables with the same name without alias
                    // so this behavior is fine
                    return DatabaseObjectName;
                }
            }
        }

        public List<ColumnReference> ColumnReferences
        {
            get { return columnReferences; }
        }

#endregion
        #region Constructors and initializer

        public TableReference()
        {
            InitializeMembers();
        }

        public TableReference(TableReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        public TableReference(string alias)
        {
            this.alias = alias;
            this.tableContext = TableContext.None;
            this.isComputed = false;
            this.isResolved = false;
        }

        public TableReference(TableOrView table, string alias, bool copyColumns)
            : base(table)
        {
            this.alias = alias;
            this.tableContext = TableContext.None;
            this.isComputed = false;
            this.isResolved = true;

            this.columnReferences = new List<ColumnReference>();

            if (copyColumns)
            {
                foreach (var c in table.Columns.Values)
                {
                    columnReferences.Add(new ColumnReference(c, this, new DataTypeReference(c.DataType)));
                }
            }
        }

        public TableReference(QueryExpression qe)
        {
            InitializeMembers();

            this.Node = qe;
        }

        public TableReference(QuerySpecification qs)
        {
            InitializeMembers();

            this.Node = qs;
        }

        public TableReference(VariableTableSource ts)
        {
            InterpretTableSource(ts);

            this.Node = ts;
        }

        public TableReference(SubqueryTableSource ts)
            : this()
        {
            InterpretTableSource(ts);

            this.Node = ts;
        }

        public TableReference(CommonTableSpecification ts)
            : this()
        {
            InterpretTableSource(ts);

            this.Node = ts;
        }

        public TableReference(ColumnIdentifier ci)
            : this()
        {
            InterpretColumnIdentifier(ci);

            this.Node = ci;
        }

        public TableReference(TableOrViewName ti)
            : this()
        {
            InterpretTableOrViewName(ti);

            this.Node = ti;
        }

        public TableReference(TableValuedFunctionCall tvf)
            : this()
        {
            InterpretTableValuedFunctionCall(tvf);

            this.Node = tvf;
        }

        public TableReference(UserVariable v)
            :this()
        {
            InterpretUserVariable(v);

            this.Node = v;
        }

        private void InitializeMembers()
        {
            this.alias = null;
            this.tableContext = TableContext.None;
            this.isComputed = false;
            this.isResolved = false;

            this.columnReferences = new List<ColumnReference>();
        }

        private void CopyMembers(TableReference old)
        {
            this.alias = old.alias;
            this.tableContext = old.tableContext;
            this.isComputed = old.isComputed;
            this.isResolved = old.isResolved;

            // Deep copy of column references
            this.columnReferences = new List<ColumnReference>();

            foreach (var cr in old.columnReferences)
            {
                var ncr = new ColumnReference(cr)
                {
                    TableReference = this
                };
                this.columnReferences.Add(ncr);
            }
        }

        public override object Clone()
        {
            return new TableReference(this);
        }

#endregion

        public void InterpretTableSource(Node tableSource)
        {
            Node = tableSource.FindAscendant<TableSource>();

            TableAlias a = tableSource.FindDescendant<TableAlias>();
            if (a != null)
            {
                alias = Util.RemoveIdentifierQuotes(a.Value);
            }
            else
            {
                alias = null;
            }

            if (tableSource is SubqueryTableSource ||
                tableSource is CommonTableSpecification)
            {
                InterpretSubquery();
            }
        }

        private void InterpretColumnIdentifier(ColumnIdentifier ci)
        {
            var ds = ci.FindDescendant<DatasetName>();
            DatasetName = (ds != null) ? Util.RemoveIdentifierQuotes(ds.Value) : null;

            var dbn = ci.FindDescendant<DatabaseName>();
            DatabaseName = (dbn != null) ? Util.RemoveIdentifierQuotes(dbn.Value) : null;

            var sn = ci.FindDescendant<SchemaName>();
            SchemaName = (sn != null) ? Util.RemoveIdentifierQuotes(sn.Value) : null;

            var tn = ci.FindDescendant<TableName>();
            DatabaseObjectName = (tn != null) ? Util.RemoveIdentifierQuotes(tn.Value) : null;
        }

        private void InterpretTableOrViewName(TableOrViewName ti)
        {
            var ds = ti.FindDescendant<DatasetName>();
            DatasetName = (ds != null) ? Util.RemoveIdentifierQuotes(ds.Value) : null;

            var dbn = ti.FindDescendant<DatabaseName>();
            DatabaseName = (dbn != null) ? Util.RemoveIdentifierQuotes(dbn.Value) : null;

            var sn = ti.FindDescendant<SchemaName>();
            SchemaName = (sn != null) ? Util.RemoveIdentifierQuotes(sn.Value) : null;

            var tn = ti.FindDescendant<TableName>();
            DatabaseObjectName = (tn != null) ? Util.RemoveIdentifierQuotes(tn.Value) : null;

            this.tableContext |= TableContext.TableOrView;
        }

        private void InterpretTableValuedFunctionCall(TableValuedFunctionCall tvf)
        {
            var fi = tvf.FindDescendant<FunctionIdentifier>();

            var udfi = fi.FindDescendant<UdfIdentifier>();

            if (udfi != null)
            {
                var ds = udfi.FindDescendant<DatasetName>();
                DatasetName = (ds != null) ? Util.RemoveIdentifierQuotes(ds.Value) : null;

                var dbn = udfi.FindDescendant<DatabaseName>();
                DatabaseName = (dbn != null) ? Util.RemoveIdentifierQuotes(dbn.Value) : null;

                var sn = udfi.FindDescendant<SchemaName>();
                SchemaName = (sn != null) ? Util.RemoveIdentifierQuotes(sn.Value) : null;

                var tn = udfi.FindDescendant<FunctionName>();
                DatabaseObjectName = (tn != null) ? Util.RemoveIdentifierQuotes(tn.Value) : null;

                tableContext |= TableContext.UserDefinedFunction;
            }
            else
            {
                throw new NameResolverException(ExceptionMessages.FunctionCallNotAllowed);
            }
        }

        private void InterpretUserVariable(UserVariable v)
        {
            // TODO: add variable name?
            tableContext |= TableContext.Variable;
        }

        public void InterpretTableDefinition(TableDefinitionList tableDefinition)
        {
            foreach (var item in tableDefinition.EnumerateTableDefinitionItems())
            {
                var cd = item.ColumnDefinition;
                var tc = item.TableConstraint;

                if (cd != null)
                {
                    var cr = cd.ColumnReference;
                    cr.TableReference = this;
                    this.ColumnReferences.Add(cr);
                }

                if (item.TableConstraint != null)
                {
                    // TODO: implement, if index name resolution is required
                }
            }
        }

        private void InterpretSubquery()
        {
            this.tableContext |= TableContext.Subquery;
            this.isComputed = false;
        }

        public void LoadColumnReferences(SchemaManager schemaManager)
        {
            this.columnReferences.Clear();

            if (tableContext.HasFlag(TableContext.CommonTable) ||
                tableContext.HasFlag(TableContext.Subquery))
            {
                throw new InvalidOperationException();
            }
            else if (tableContext.HasFlag(TableContext.UserDefinedFunction))
            {
                LoadUdfColumnReferences(schemaManager);
            }
            else if (tableContext.HasFlag(TableContext.TableOrView))
            {
                LoadTableOrViewColumnReferences(schemaManager);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void LoadUdfColumnReferences(SchemaManager schemaManager)
        {
            // TVF calls can have a column alias list
            List<ColumnAlias> calist = null;
            var cal = this.Node.FindDescendant<ColumnAliasList>();
            if (cal != null)
            {
                calist = new List<ColumnAlias>(cal.EnumerateDescendants<ColumnAlias>());
            }

            // Get dataset description
            DatasetBase ds;
            try
            {
                ds = schemaManager.Datasets[DatasetName];
            }
            catch (SchemaException ex)
            {
                throw new NameResolverException(String.Format(ExceptionMessages.UnresolvableDatasetReference, DatasetName, Node.Line, Node.Col), ex);
            }

            int q = 0;
            TableValuedFunction tvf;
            if (ds.TableValuedFunctions.ContainsKey(DatabaseName, SchemaName, DatabaseObjectName))
            {
                tvf = ds.TableValuedFunctions[DatabaseName, SchemaName, DatabaseObjectName];
            }
            else
            {
                // TODO: move this to name resolver instead
                throw new NameResolverException(String.Format(ExceptionMessages.UnresolvableUdfReference, DatabaseObjectName, Node.Line, Node.Col));
            }

            foreach (var cd in tvf.Columns.Values)
            {
                var cr = new ColumnReference(cd, this, new DataTypeReference(cd.DataType));

                // if column alias list is present, use the alias instead of the original name
                if (calist != null)
                {
                    cr.ColumnName = Util.RemoveIdentifierQuotes(calist[q].Value);
                }

                this.columnReferences.Add(cr);
                q++;
            }
        }

        private void LoadTableOrViewColumnReferences(SchemaManager schemaManager)
        {
            // Get dataset description
            DatasetBase ds;
            try
            {
                ds = schemaManager.Datasets[DatasetName];
            }
            catch (SchemaException ex)
            {
                throw new NameResolverException(String.Format(ExceptionMessages.UnresolvableDatasetReference, DatasetName, Node.Line, Node.Col), ex);
            }

            // Get table description
            TableOrView td;
            if (ds.Tables.ContainsKey(DatabaseName, SchemaName, DatabaseObjectName))
            {
                td = ds.Tables[DatabaseName, SchemaName, DatabaseObjectName];
            }
            else if (ds.Views.ContainsKey(DatabaseName, SchemaName, DatabaseObjectName))
            {
                td = ds.Views[DatabaseName, SchemaName, DatabaseObjectName];
            }
            else
            {
                throw new NameResolverException(String.Format(ExceptionMessages.UnresolvableTableReference, DatabaseObjectName, Node.Line, Node.Col));
            }

            // Copy columns to the table reference in appropriate order
            this.columnReferences.AddRange(td.Columns.Values.OrderBy(c => c.ID).Select(c => new ColumnReference(c, this, new DataTypeReference(c.DataType))));
        }

        /// <summary>
        /// Compares two table references for name resolution.
        /// </summary>
        /// <remarks>
        /// It is a logical comparison that follows the rules of name resolution logic
        /// in queries.
        /// </remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Compare(TableReference other)
        {
            // If object are the same
            if (this == other)
            {
                return true;
            }

            // Otherwise compare strings
            bool res = true;

            res = res && (this.DatasetName == null || other.DatasetName == null ||
                    SchemaManager.Comparer.Compare(this.DatasetName, other.DatasetName) == 0);

            res = res && (this.DatabaseName == null || other.DatabaseName == null ||
                    SchemaManager.Comparer.Compare(this.DatabaseName, other.DatabaseName) == 0);

            res = res && (this.SchemaName == null || other.SchemaName == null ||
                    SchemaManager.Comparer.Compare(this.SchemaName, other.SchemaName) == 0);

            res = res && (this.DatabaseObjectName == null || other.DatabaseObjectName == null ||
                    SchemaManager.Comparer.Compare(this.DatabaseObjectName, other.DatabaseObjectName) == 0);

            // When resolving columns, a table reference of a column may match any table or alias
            // if no alias, nor table name is specified but
            // the two aliases, if specified, must always match

            res = res &&
                (this.DatasetName == null && this.DatabaseName == null && this.SchemaName == null && this.DatabaseObjectName == null && this.alias == null ||
                 other.DatasetName == null && other.DatabaseName == null && other.SchemaName == null && other.DatabaseObjectName == null && other.alias == null ||
                 this.alias == null && other.alias == null ||
                 this.alias != null && other.alias != null && SchemaManager.Comparer.Compare(this.alias, other.alias) == 0);

            return res;
        }

        public List<ColumnReference> FilterColumnReferences(ColumnContext columnContext)
        {
            var res = new Dictionary<string, ColumnReference>();
            var t = (TableOrView)DatabaseObject;            // TODO: what if function?

            // Primary key columns
            if ((columnContext & ColumnContext.PrimaryKey) != 0 && t.PrimaryKey != null)
            {
                foreach (var cd in t.PrimaryKey.Columns.Values)
                {
                    if (!res.ContainsKey(cd.ColumnName))
                    {
                        res.Add(cd.ColumnName, new ColumnReference(cd, this, new DataTypeReference(cd.DataType)));
                    }
                }
            }

            // Columns marked as key
            if ((columnContext & ColumnContext.Key) != 0)
            {
                foreach (var cd in t.Columns.Values)
                {
                    if (cd.IsKey && !res.ContainsKey(cd.ColumnName))
                    {
                        res.Add(cd.ColumnName, new ColumnReference(cd, this, new DataTypeReference(cd.DataType)));
                    }
                }
            }

            // Other columns
            foreach (var cr in ColumnReferences)
            {
                // Avoid hint and special contexts
                if (((columnContext & cr.ColumnContext) != 0 || (columnContext & ColumnContext.NonReferenced) != 0)
                    && !res.ContainsKey(cr.ColumnName))
                {
                    res.Add(cr.ColumnName, cr);
                }
            }

            return new List<ColumnReference>(res.Values.OrderBy(c => t.Columns[c.ColumnName].ID));
        }

    }
}
