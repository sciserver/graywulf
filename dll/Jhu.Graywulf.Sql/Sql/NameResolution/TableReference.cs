using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class TableReference : DatabaseObjectReference, IColumnReferences
    {
        #region Property storage variables

        private string alias;
        private string variableName;
        private TableContext tableContext;
        private bool isComputed;
        private TableSource tableSource;
        private VariableReference variableReference;
        private IndexedDictionary<string, ColumnReference> columnReferences;

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

        public string TableName
        {
            get { return DatabaseObjectName; }
            set { DatabaseObjectName = value; }
        }

        public string VariableName
        {
            get { return variableName; }
            set { variableName = value; }
        }

        public TableContext TableContext
        {
            get { return tableContext; }
            set { tableContext = value; }
        }

        public TableOrView TableOrView
        {
            get { return (TableOrView)DatabaseObject; }
            set { DatabaseObject = value; }
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

        public bool IsCachable
        {
            get
            {
                return
                  !tableContext.HasFlag(TableContext.Subquery) &&
                  !tableContext.HasFlag(TableContext.CommonTable) &&
                  !tableContext.HasFlag(TableContext.UserDefinedFunction) &&
                  !tableContext.HasFlag(TableContext.Variable) &&
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
                return (alias != null || DatabaseObjectName != null) && DatasetName == null && DatabaseName == null && VariableName == null;
            }
        }

        public override bool IsUndefined
        {
            get { return base.IsUndefined && alias == null && variableName == null; }
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
                // TODO: review this and make sure key is unique even if table
                // is referenced deep down in CTEs

                if (!String.IsNullOrWhiteSpace(variableName))
                {
                    return String.Format("{0}", variableName);
                }
                else if (!String.IsNullOrWhiteSpace(alias))
                {
                    return String.Format("[{0}]", alias);
                }
                else
                {
                    return base.UniqueName;
                }
            }
        }

        public TableSource TableSource
        {
            get { return tableSource; }
            set { tableSource = value; }
        }

        public VariableReference VariableReference
        {
            get { return variableReference; }
            set { variableReference = value; }
        }

        public IndexedDictionary<string, ColumnReference> ColumnReferences
        {
            get { return columnReferences; }
        }

        #endregion
        #region Constructors and initializer

        public TableReference()
        {
            InitializeMembers();
        }

        public TableReference(Node node)
            : base(node)
        {
            InitializeMembers();
        }

        public TableReference(string alias)
        {
            InitializeMembers();

            this.alias = alias;
        }

        public TableReference(TableOrView table, string alias, bool copyColumns)
            : base(table)
        {
            InitializeMembers();

            this.alias = alias;

            if (copyColumns)
            {
                foreach (var c in table.Columns.Values)
                {
                    columnReferences.Add(c.ColumnName, new ColumnReference(c, this, new DataTypeReference(c.DataType)));
                }
            }
        }

        public TableReference(TableReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.alias = null;
            this.variableName = null;
            this.tableContext = TableContext.None;
            this.isComputed = false;
            this.tableSource = null;
            this.variableReference = null;
            this.columnReferences = new IndexedDictionary<string, ColumnReference>(SchemaManager.Comparer);
        }

        private void CopyMembers(TableReference old)
        {
            this.alias = old.alias;
            this.variableName = old.variableName;
            this.tableContext = old.tableContext;
            this.isComputed = old.isComputed;
            this.tableSource = old.tableSource;
            this.variableReference = old.variableReference;

            // Deep copy of column references
            this.columnReferences = new IndexedDictionary<string, ColumnReference>(SchemaManager.Comparer);
            foreach (var key in old.columnReferences.Keys)
            {
                var ncr = new ColumnReference(this, old.columnReferences[key]);
                this.columnReferences.Add(key, ncr);
            }
        }

        public override object Clone()
        {
            return new TableReference(this);
        }

        #endregion

        public static TableReference Interpret(FunctionTableSource ts)
        {
            var alias = ts.Alias;
            var fr = ts.FunctionReference;

            var tr = new TableReference(ts)
            {
                alias = RemoveIdentifierQuotes(alias?.Value),
                tableSource = ts,
                DatasetName = fr.DatasetName,
                DatabaseName = fr.DatabaseName,
                SchemaName = fr.SchemaName,
                DatabaseObjectName = fr.DatabaseObjectName,
                tableContext = TableContext.From | TableContext.UserDefinedFunction
            };

            return tr;
        }

        public static TableReference Interpret(SimpleTableSource ts)
        {
            var tr = ts.TableReference;
            var alias = ts.Alias;

            tr.alias = RemoveIdentifierQuotes(alias?.Value);
            tr.tableSource = ts;
            tr.tableContext |= TableContext.From;

            return tr;
        }

        public static TableReference Interpret(VariableTableSource ts)
        {
            var alias = ts.Alias;
            var variable = ts.Variable;

            var tr = new TableReference(ts)
            {
                alias = RemoveIdentifierQuotes(alias?.Value ?? variable?.Value),
                tableSource = ts,
                variableName = variable.VariableName,
                variableReference = variable.VariableReference,
                tableContext = TableContext.From | TableContext.Variable
            };

            return tr;
        }

        public static TableReference Interpret(SubqueryTableSource ts)
        {
            var alias = ts.Alias;

            var tr = new TableReference(ts)
            {
                alias = RemoveIdentifierQuotes(alias.Value),
                tableSource = ts,
                tableContext = TableContext.From | TableContext.Subquery,
            };

            // TODO: is subquery parsed at this point? Copy columns now?

            return tr;
        }

        public static TableReference Interpret(CommonTableSpecification cts)
        {
            var alias = cts.Alias;
            var subquery = cts.Subquery;

            var tr = new TableReference(cts)
            {
                alias = RemoveIdentifierQuotes(alias.Value),
                tableSource = cts,
                tableContext = TableContext.Subquery | TableContext.CommonTable,
            };

            // TODO: is subquery parsed at this point? Copy columns now?
            // What about column name aliases?

            return tr;
        }

        // TODO: add derived table source logic (VALUES part with column alias list)

        public static TableReference Interpret(TableOrViewIdentifier ti)
        {
            var ds = ti.FindDescendant<DatasetPrefix>();
            var database = ti.FindDescendant<DatabaseName>()?.Value;
            var schema = ti.FindDescendant<SchemaName>()?.Value;
            var table = ti.FindDescendant<TableName>()?.Value;

            var tr = new TableReference(ti)
            {
                DatasetName = RemoveIdentifierQuotes(ds?.DatasetName),
                DatabaseName = RemoveIdentifierQuotes(database),
                SchemaName = RemoveIdentifierQuotes(schema),
                DatabaseObjectName = RemoveIdentifierQuotes(table),
                IsUserDefined = true,
            };

            return tr;
        }

        public static TableReference Interpret(TargetTableSpecification tt)
        {
            TableReference tr;
            var table = tt.TableOrViewIdentifier;
            var variable = tt.UserVariable;

            if (table != null)
            {
                tr = table.TableReference;
                tr.tableSource = tt;
            }
            else if (variable != null)
            {
                tr = new TableReference()
                {
                    tableSource = tt,
                    variableName = variable.VariableName,
                    variableReference = variable.VariableReference,
                    tableContext = TableContext.Target | TableContext.Variable
                };
            }
            else
            {
                throw new NotImplementedException();
            }

            return tr;
        }

        public static TableReference Interpret(TokenList tokens, int colpart)
        {
            switch (colpart)
            {
                case 0:
                    return null;
                case 1:
                    return new TableReference((Node)tokens[0])
                    {
                        TableName = RemoveIdentifierQuotes(tokens[0].Value),
                    };
                case 2:
                    return new TableReference((Node)tokens[0])
                    {
                        SchemaName = RemoveIdentifierQuotes(tokens[0].Value),
                        TableName = RemoveIdentifierQuotes(tokens[1].Value),
                    };
                case 3:
                    return new TableReference((Node)tokens[0])
                    {
                        DatabaseName = RemoveIdentifierQuotes(tokens[0].Value),
                        SchemaName = RemoveIdentifierQuotes(tokens[1].Value),
                        TableName = RemoveIdentifierQuotes(tokens[2].Value),
                    };
                default:
                    throw new InvalidOperationException();
            }
        }

        public override void LoadDatabaseObject(DatasetBase dataset)
        {
            if (tableContext.HasFlag(TableContext.CommonTable) ||
                tableContext.HasFlag(TableContext.Subquery) ||
                tableContext.HasFlag(TableContext.Variable))
            {
                throw new InvalidOperationException();
            }
            else if (tableContext.HasFlag(TableContext.UserDefinedFunction))
            {
                LoadTableValuedFunction(dataset);
            }
            else if (tableContext.HasFlag(TableContext.TableOrView) ||
                tableContext.HasFlag(TableContext.Target))
            {
                LoadTableOrView(dataset);
            }
            else
            {
                LoadTableOrView(dataset);

                if (DatabaseObject == null)
                {
                    LoadTableValuedFunction(dataset);
                }
            }
        }

        private void LoadTableOrView(DatasetBase dataset)
        {
            if (dataset.Tables.ContainsKey(DatabaseName, SchemaName, DatabaseObjectName))
            {
                DatabaseObject = dataset.Tables[DatabaseName, SchemaName, DatabaseObjectName];
                tableContext |= TableContext.TableOrView;
            }
            else if (dataset.Views.ContainsKey(DatabaseName, SchemaName, DatabaseObjectName))
            {
                DatabaseObject = dataset.Views[DatabaseName, SchemaName, DatabaseObjectName];
                tableContext |= TableContext.TableOrView;
            }
        }

        private void LoadTableValuedFunction(DatasetBase dataset)
        {
            if (dataset.TableValuedFunctions.ContainsKey(DatabaseName, SchemaName, DatabaseObjectName))
            {
                DatabaseObject = dataset.TableValuedFunctions[DatabaseName, SchemaName, DatabaseObjectName];
                tableContext |= TableContext.UserDefinedFunction;
            }
        }

        public void CopyColumnReferences(IEnumerable<IColumnReference> other)
        {
            CopyColumnReferences(other.Select(cr => cr.ColumnReference));
        }

        public void CopyColumnReferences(IEnumerable<ColumnReference> other)
        {
            this.columnReferences.Clear();

            foreach (var cr in other)
            {
                var ncr = new ColumnReference(cr)
                {
                    TableReference = this
                };
                this.columnReferences.Add(ncr.ColumnName, ncr);
            }
        }

        public void LoadColumnReferences()
        {
            this.columnReferences.Clear();

            if (tableContext.HasFlag(TableContext.CommonTable) ||
                tableContext.HasFlag(TableContext.Subquery))
            {
                throw new InvalidOperationException();
            }
            else if (DatabaseObject is TableValuedFunction ||
                tableContext.HasFlag(TableContext.UserDefinedFunction))
            {
                LoadUdfColumnReferences();
            }
            else if (tableContext.HasFlag(TableContext.Variable))
            {
                CopyColumnReferences(variableReference.DataTypeReference.ColumnReferences);
            }
            else if (DatabaseObject is TableOrView ||
                tableContext.HasFlag(TableContext.TableOrView) ||
                tableContext.HasFlag(TableContext.Target))
            {
                LoadTableOrViewColumnReferences();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void LoadUdfColumnReferences()
        {
            int q = 0;
            foreach (var cd in ((TableValuedFunction)DatabaseObject).Columns.Values)
            {
                var cr = new ColumnReference(cd, this, new DataTypeReference(cd.DataType));
                columnReferences.Add(cr.ColumnName, cr);
                q++;
            }
        }

        private void LoadTableOrViewColumnReferences()
        {
            // Copy columns to the table reference in appropriate order
            var table = (TableOrView)DatabaseObject;
            foreach (var c in table.Columns.Values.OrderBy(i => i.ID))
            {
                columnReferences.Add(c.ColumnName, new ColumnReference(c, this, new DataTypeReference(c.DataType)));
            }
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
        public bool TryMatch(TableReference other)
        {
            // If object are the same reference
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

            // TODO: use exported name here instead of alias?
            // this needs testing

            res = res &&
                (this.IsUndefined || other.IsUndefined ||
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
                if ((columnContext & cr.ColumnContext) != 0
                    && !res.ContainsKey(cr.ColumnName))
                {
                    res.Add(cr.ColumnName, cr);
                }
            }

            return new List<ColumnReference>(res.Values.OrderBy(c => t.Columns[c.ColumnName].ID));
        }
    }
}
