using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlParser
{
    public class TableReference
    {
        #region Property storage variables

        private Node node;

        private DatabaseObject databaseObject;

        private string datasetName;
        private string databaseName;
        private string schemaName;
        private string databaseObjectName;
        private string alias;

        private bool isTableOrView;
        private bool isUdf;
        private bool isSubquery;
        private bool isComputed;

        private List<ColumnReference> columnReferences;
        //private List<SearchConditionReference> conditionReferences;

        private TableStatistics statistics;

        #endregion

        /// <summary>
        /// Gets the parser tree node this table reference references
        /// </summary>
        public Node Node
        {
            get { return node; }
        }

        /// <summary>
        /// Gets or set the database object (schema object) this reference refers to
        /// </summary>
        public DatabaseObject DatabaseObject
        {
            get { return databaseObject; }
            set { databaseObject = value; }
        }

        /// <summary>
        /// Gets or sets the resolved dataset name
        /// </summary>
        public string DatasetName
        {
            get { return datasetName; }
            set { datasetName = value; }
        }

        /// <summary>
        /// Gets or sets the resolved database name
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        /// <summary>
        /// Gets or sets the resolved schema name
        /// </summary>
        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }

        /// <summary>
        /// Gets or sets the resolved object name
        /// </summary>
        public string DatabaseObjectName
        {
            get { return databaseObjectName; }
            set { databaseObjectName = value; }
        }

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

        public bool IsUndefined
        {
            get { return datasetName == null && databaseName == null && schemaName == null && databaseObjectName == null && alias == null; }
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
                return (alias != null || databaseObjectName != null) && datasetName == null && databaseName == null;
            }
        }

        /// <summary>
        /// Gets the fully qualified name of the table or view in the : notation.
        /// </summary>
        public string FullyQualifiedName
        {
            get
            {
                if (isSubquery || isComputed)
                {
                    return String.Format("[{0}]", alias);
                }
                else
                {
                    string res = String.Empty;

                    // If it's not resolved yet
                    if (datasetName != null) res += String.Format("[{0}]:", datasetName);
                    if (databaseName != null) res += String.Format("[{0}].", databaseName);
                    if (schemaName != null) res += String.Format("[{0}].", schemaName);
                    if (databaseObjectName != null) res += String.Format("[{0}]", databaseObjectName);

                    return res;
                }
            }
        }

        /// <summary>
        /// Gets the unique name of the table (alias, if available)
        /// </summary>
        public string UniqueName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(alias))
                {
                    return FullyQualifiedName;
                }
                else
                {
                    return Alias;
                }
            }
        }

        public string EscapedUniqueName
        {
            get
            {
                if (isSubquery || isComputed)
                {
                    return alias;
                }
                else
                {
                    string res = String.Empty;

                    // If it's not resolved yet
                    if (datasetName != null) res += String.Format("{0}_", Util.EscapeIdentifierName(datasetName));
                    if (databaseName != null) res += String.Format("{0}_", Util.EscapeIdentifierName(databaseName));
                    if (schemaName != null) res += String.Format("{0}_", Util.EscapeIdentifierName(schemaName));
                    if (databaseObjectName != null) res += String.Format("{0}", Util.EscapeIdentifierName(databaseObjectName));

                    return res;
                }
            }
        }

        public List<ColumnReference> ColumnReferences
        {
            get { return columnReferences; }
        }

        //public List<SearchConditionReference> ConditionReferences
        //{
        //    get { return conditionReferences; }
        //}

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
        {
            CopyMembers(old);
        }

        public TableReference(QueryExpression qe)
        {
            InitializeMembers();

            this.node = qe;
        }

        public TableReference(QuerySpecification qs)
        {
            InitializeMembers();

            this.node = qs;
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
            this.node = null;

            this.databaseObject = null;

            this.datasetName = null;
            this.databaseName = null;
            this.schemaName = null;
            this.databaseObjectName = null;
            this.alias = null;

            this.isTableOrView = false;
            this.isUdf = false;
            this.isSubquery = false;
            this.isComputed = false;

            this.columnReferences = new List<ColumnReference>();
            //this.conditionReferences = new List<SearchConditionReference>();

            this.statistics = null;
        }

        private void CopyMembers(TableReference old)
        {
            this.node = old.node;

            this.databaseObject = old.databaseObject;

            this.datasetName = old.datasetName;
            this.databaseName = old.databaseName;
            this.schemaName = old.schemaName;
            this.databaseObjectName = old.databaseObjectName;
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
            //this.conditionReferences = new List<SearchConditionReference>(old.conditionReferences);

            this.statistics = old.statistics == null ? null : new TableStatistics(old.statistics);
        }

        internal void InterpretTableSource(Node tableSource)
        {
            node = tableSource.FindAscendant<TableSource>();

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
            datasetName = (ds != null) ? Util.RemoveIdentifierQuotes(ds.Value) : null;

            var dbn = ci.FindDescendant<DatabaseName>();
            databaseName = (dbn != null) ? Util.RemoveIdentifierQuotes(dbn.Value) : null;

            var sn = ci.FindDescendant<SchemaName>();
            schemaName = (sn != null) ? Util.RemoveIdentifierQuotes(sn.Value) : null;

            var tn = ci.FindDescendant<TableName>();
            databaseObjectName = (tn != null) ? Util.RemoveIdentifierQuotes(tn.Value) : null;
        }

        private void InterpretTableOrViewName(TableOrViewName ti)
        {
            var ds = ti.FindDescendant<DatasetName>();
            datasetName = (ds != null) ? Util.RemoveIdentifierQuotes(ds.Value) : null;

            var dbn = ti.FindDescendant<DatabaseName>();
            databaseName = (dbn != null) ? Util.RemoveIdentifierQuotes(dbn.Value) : null;

            var sn = ti.FindDescendant<SchemaName>();
            schemaName = (sn != null) ? Util.RemoveIdentifierQuotes(sn.Value) : null;

            var tn = ti.FindDescendant<TableName>();
            databaseObjectName = (tn != null) ? Util.RemoveIdentifierQuotes(tn.Value) : null;

            isTableOrView = true;
        }

        private void InterpretTableValuedFunctionCall(TableValuedFunctionCall tvf)
        {
            var fi = tvf.FindDescendant<FunctionIdentifier>();

            var udfi = fi.FindDescendant<UdfIdentifier>();

            if (udfi != null)
            {
                var ds = udfi.FindDescendant<DatasetName>();
                datasetName = (ds != null) ? Util.RemoveIdentifierQuotes(ds.Value) : null;

                var dbn = udfi.FindDescendant<DatabaseName>();
                databaseName = (dbn != null) ? Util.RemoveIdentifierQuotes(dbn.Value) : null;

                var sn = udfi.FindDescendant<SchemaName>();
                schemaName = (sn != null) ? Util.RemoveIdentifierQuotes(sn.Value) : null;

                var tn = udfi.FindDescendant<FunctionName>();
                databaseObjectName = (tn != null) ? Util.RemoveIdentifierQuotes(tn.Value) : null;

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

        /// <summary>
        /// Substitute default dataset and schema names, if necessary
        /// </summary>
        /// <param name="defaultDataSetName"></param>
        /// <param name="defaultSchemaName"></param>
        public void SubstituteDefaults(SchemaManager schemaManager, string defaultDataSetName)
        {
            // This cannot be called for subqueries

            if (this.datasetName == null)
            {
                this.datasetName = defaultDataSetName;
            }

            if (this.schemaName == null)
            {
                this.schemaName = schemaManager.Datasets[this.datasetName].DefaultSchemaName;
            }
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
            var cal = this.node.FindDescendant<ColumnAliasList>();
            if (cal != null)
            {
                calist = new List<ColumnAlias>(cal.EnumerateDescendants<ColumnAlias>());
            }

            // Get dataset description
            DatasetBase ds;
            try
            {
                ds = schemaManager.Datasets[datasetName];
            }
            catch (SchemaException ex)
            {
                throw new NameResolverException(String.Format(ExceptionMessages.UnresolvableDatasetReference, datasetName, node.Line, node.Col), ex);
            }

            int q = 0;
            TableValuedFunction tvf;
            if (ds.TableValuedFunctions.ContainsKey(databaseName, schemaName, databaseObjectName))
            {
                tvf = ds.TableValuedFunctions[databaseName, schemaName, databaseObjectName];
            }
            else
            {
                // TODO: move this to name resolver instead
                throw new NameResolverException(String.Format(ExceptionMessages.UnresolvableUdfReference, databaseObjectName, node.Line, node.Col));
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
                ds = schemaManager.Datasets[datasetName];
            }
            catch (SchemaException ex)
            {
                throw new NameResolverException(String.Format(ExceptionMessages.UnresolvableDatasetReference, datasetName, node.Line, node.Col), ex);
            }

            // Get table description
            TableOrView td;
            if (ds.Tables.ContainsKey(databaseName, schemaName, databaseObjectName))
            {
                //td = new Table(ds.Tables[databaseName, schemaName, databaseObjectName]);
                td = ds.Tables[databaseName, schemaName, databaseObjectName];
            }
            else if (ds.Views.ContainsKey(databaseName, schemaName, databaseObjectName))
            {
                //td = new View(ds.Views[databaseName, schemaName, databaseObjectName]);
                td = ds.Views[databaseName, schemaName, databaseObjectName];
            }
            else
            {
                // TODO: move this to name resolver instead
                throw new NameResolverException(String.Format(ExceptionMessages.UnresolvableTableReference, databaseObjectName, node.Line, node.Col));
            }

            // Copy columns to the table reference in appropriate order
            this.columnReferences.AddRange(td.Columns.Values.OrderBy(c => c.ID).Select(c => new ColumnReference(this, c)));
        }

        public bool Compare(TableReference other)
        {
            bool res = true;

            res &= (this.datasetName == null || other.datasetName == null ||
                    SchemaManager.Comparer.Compare(this.datasetName, other.datasetName) == 0);

            res &= (this.databaseName == null || other.databaseName == null ||
                    SchemaManager.Comparer.Compare(this.databaseName, other.databaseName) == 0);

            res &= (this.schemaName == null || other.schemaName == null ||
                    SchemaManager.Comparer.Compare(this.schemaName, other.schemaName) == 0);

            res &= (this.databaseObjectName == null || other.databaseObjectName == null ||
                    SchemaManager.Comparer.Compare(this.databaseObjectName, other.databaseObjectName) == 0);

            res &= (this.alias == null || other.alias == null ||
                    SchemaManager.Comparer.Compare(this.alias, other.alias) == 0);

            return res;
        }

        /// <summary>
        /// Gets the fully resolved three part name of the table or view.
        /// </summary>
        /// <remarks>
        /// The fully resolved name is in the dbname.schema.tablename format.
        /// </remarks>
        public string GetFullyResolvedName()
        {
            if (isSubquery || isComputed)
            {
                return String.Format("[{0}]", alias);
            }
            else
            {
                // If it is linked up to the schema, return
                if (databaseObject != null)
                {
                    return databaseObject.GetFullyResolvedName();
                }
                else
                {
                    return FullyQualifiedName;
                }
            }
        }

        public override string ToString()
        {
            if (IsSubquery)
            {
                return String.Format("[subquery] AS {0}", alias);
            }
            else if (alias != null)
            {
                return String.Format("{0} AS {1}", GetFullyResolvedName(), alias);
            }
            else
            {
                return GetFullyResolvedName();
            }
        }
    }
}
