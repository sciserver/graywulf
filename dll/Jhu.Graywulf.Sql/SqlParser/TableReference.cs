using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlParser
{
    public class TableReference : DatabaseObjectReference
    {
        #region Property storage variables
        
        private string alias;

        private bool isTableOrView;
        private bool isUdf;
        private bool isSubquery;
        private bool isComputed;

        private List<ColumnReference> columnReferences;
        private TableStatistics statistics;

        #endregion

        /// <summary>
        /// Gets or sets the resolved alias
        /// </summary>
        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }

        /// <summary>
        /// Gets the value indicating whether the table source is a table or view
        /// </summary>
        public bool IsTableOrView
        {
            get { return isTableOrView; }
        }

        /// <summary>
        /// Gets the value indicating whether the table source is a table valued function
        /// </summary>
        public bool IsUdf
        {
            get { return isUdf; }
        }

        public bool IsSubquery
        {
            get { return isSubquery; }
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
            protected set { isComputed = value; }
        }

        public bool IsCachable
        {
            get { return !IsSubquery && !IsUdf && !IsComputed; }
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

        public List<ColumnReference> ColumnReferences
        {
            get { return columnReferences; }
        }

        public TableStatistics Statistics
        {
            get { return statistics; }
            set { statistics = value; }
        }

        public TableReference()
        {
            InitializeMembers();
        }

        public TableReference(TableReference old)
            : base(old)
        {
            CopyMembers(old);
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
            throw new NotImplementedException();
        }

        public TableReference(SubqueryTableSource ts)
        {
            InitializeMembers();
            InterpretTableSource(ts);
            InterpretSubquery();
        }

        public TableReference(ColumnIdentifier ci)
            : this()
        {
            InitializeMembers();
            InterpretColumnIdentifier(ci);
        }

        public TableReference(TableOrViewName ti)
            : this()
        {
            InitializeMembers();
            InterpretTableOrViewName(ti);
        }

        public TableReference(TableValuedFunctionCall tvf)
            : this()
        {
            InitializeMembers();
            InterpretTableValuedFunctionCall(tvf);
        }

        private void InitializeMembers()
        {
            this.alias = null;

            this.isTableOrView = false;
            this.isUdf = false;
            this.isSubquery = false;
            this.isComputed = false;

            this.columnReferences = new List<ColumnReference>();
            this.statistics = null;
        }

        private void CopyMembers(TableReference old)
        {
            this.alias = old.alias;

            this.isTableOrView = old.isTableOrView;
            this.isUdf = old.isUdf;
            this.isSubquery = old.isSubquery;
            this.isComputed = old.isComputed;

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
            this.statistics = old.statistics == null ? null : new TableStatistics(old.statistics);
        }

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

            isTableOrView = true;
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

                isUdf = true;
            }
            else
            {
                throw new NameResolverException(ExceptionMessages.FunctionCallNotAllowed);
            }
        }

        private void InterpretSubquery()
        {
            isSubquery = true;
        }

        public void LoadColumnReferences(SchemaManager schemaManager)
        {
            this.columnReferences.Clear();

            if (this.IsSubquery)
            {
                // In case of a subquery

                throw new InvalidOperationException();
            }
            else if (this.IsUdf)
            {
                // In case of a user-defined function

                LoadUdfColumnReferences(schemaManager);
            }
            else if (this.IsTableOrView)
            {
                // In case of a table

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
                var cr = new ColumnReference(this, cd);

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
                //td = new Table(ds.Tables[databaseName, schemaName, databaseObjectName]);
                td = ds.Tables[DatabaseName, SchemaName, DatabaseObjectName];
            }
            else if (ds.Views.ContainsKey(DatabaseName, SchemaName, DatabaseObjectName))
            {
                //td = new View(ds.Views[databaseName, schemaName, databaseObjectName]);
                td = ds.Views[DatabaseName, SchemaName, DatabaseObjectName];
            }
            else
            {
                // TODO: move this to name resolver instead
                throw new NameResolverException(String.Format(ExceptionMessages.UnresolvableTableReference, DatabaseObjectName, Node.Line, Node.Col));
            }

            // Copy columns to the table reference in appropriate order
            this.columnReferences.AddRange(td.Columns.Values.OrderBy(c => c.ID).Select(c => new ColumnReference(this, c)));
        }

        public bool Compare(TableReference other)
        {
            bool res = true;

            res &= (this.DatasetName == null || other.DatasetName == null ||
                    SchemaManager.Comparer.Compare(this.DatasetName, other.DatasetName) == 0);

            res &= (this.DatabaseName == null || other.DatabaseName == null ||
                    SchemaManager.Comparer.Compare(this.DatabaseName, other.DatabaseName) == 0);

            res &= (this.SchemaName == null || other.SchemaName == null ||
                    SchemaManager.Comparer.Compare(this.SchemaName, other.SchemaName) == 0);

            res &= (this.DatabaseObjectName == null || other.DatabaseObjectName == null ||
                    SchemaManager.Comparer.Compare(this.DatabaseObjectName, other.DatabaseObjectName) == 0);

            res &= (this.alias == null || other.alias == null ||
                    SchemaManager.Comparer.Compare(this.alias, other.alias) == 0);

            return res;
        }
    }
}
