using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.SqlParser
{
    public class FunctionReference
    {
        #region Property storage variables

        private Node node;

        private DatabaseObject databaseObject;

        private string datasetName;
        private string databaseName;
        private string schemaName;
        private string databaseObjectName;

        private string systemFunctionName;

        private bool isUdf;

        #endregion

        /// <summary>
        /// Gets the parser tree node this function reference
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

        public bool IsUndefined
        {
            get { return datasetName == null && databaseName == null && schemaName == null && databaseObjectName == null; }
        }

        public string SystemFunctionName
        {
            get { return systemFunctionName; }
            set { systemFunctionName = value; }
        }

        public bool IsUdf
        {
            get { return isUdf; }
        }

        public bool IsSystem
        {
            get { return !isUdf; }
        }

        /// <summary>
        /// Gets the fully qualified name of the table or view in the : notation.
        /// </summary>
        public string FullyQualifiedName
        {
            get
            {
                string res = String.Empty;

                if (IsSystem)
                {
                    res = systemFunctionName.ToUpper();
                }
                else
                {
                    // If it's not resolved yet
                    if (datasetName != null) res += String.Format("[{0}]:", datasetName);
                    if (databaseName != null) res += String.Format("[{0}].", databaseName);
                    if (schemaName != null) res += String.Format("[{0}].", schemaName);
                    if (databaseObjectName != null) res += String.Format("[{0}]", databaseObjectName);
                }

                return res;
            }
        }

        public FunctionReference()
        {
            InitializeMembers();
        }

        public FunctionReference(FunctionReference old)
        {
            CopyMembers(old);
        }

        public FunctionReference(FunctionIdentifier fi)
        {
            InitializeMembers();
            InterpretFunctionIdentifier(fi);
        }

        private void InitializeMembers()
        {
            this.node = null;

            this.databaseObject = null;

            this.datasetName = null;
            this.databaseName = null;
            this.schemaName = null;
            this.databaseObjectName = null;
            this.systemFunctionName = null;
        }

        private void CopyMembers(FunctionReference old)
        {
            this.node = old.node;

            this.databaseObject = old.databaseObject;

            this.datasetName = old.datasetName;
            this.databaseName = old.databaseName;
            this.schemaName = old.schemaName;
            this.databaseObjectName = old.databaseObjectName;
            this.systemFunctionName = old.systemFunctionName;
        }

        private void InterpretFunctionIdentifier(FunctionIdentifier fi)
        {
            var fn = fi.FunctionName;

            if (fn != null)
            {
                datasetName = null;
                databaseName = null;
                schemaName = null;
                databaseObjectName = null;

                systemFunctionName = fn.Value;

                isUdf = false;
            }
            else
            {
                var udfi = fi.UdfIdentifier;

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

                    systemFunctionName = null;

                    isUdf = true;
                }
                else
                {
                    throw new InvalidOperationException();  // *** TODO
                }
            }
        }

        /// <summary>
        /// Substitute default dataset and schema names, if necessary
        /// </summary>
        /// <param name="defaultDataSetName"></param>
        /// <param name="defaultSchemaName"></param>
        public void SubstituteDefaults(string defaultDataSetName, string defaultSchemaName)
        {
            // This cannot be called for subqueries
            if (this.datasetName == null) this.datasetName = defaultDataSetName;
            if (this.schemaName == null) this.schemaName = defaultSchemaName;
        }

        /*
         * // TODO: not used now, but might be necessary in the future
         * 
         * public bool Compare(TableReference other)
        {
            bool res = true;

            // **** verify this or delete
            //res &= (!this.isUdf && !other.isUdf) ||
            //       (this.isUdf && StringComparer.CurrentCultureIgnoreCase.Compare(this.alias, other.tableName) == 0) ||
            //       (other.isUdf && StringComparer.CurrentCultureIgnoreCase.Compare(other.alias, this.tableName) == 0) ||
            //       (this.isUdf && other.isUdf && StringComparer.CurrentCultureIgnoreCase.Compare(this.alias, other.alias) == 0);

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
        }*/

        /// <summary>
        /// Gets the fully resolved three part name of the table or view.
        /// </summary>
        /// <remarks>
        /// The fully resolved name is in the dbname.schema.tablename format.
        /// </remarks>
        public string GetFullyResolvedName()
        {
            if (IsSystem)
            {
                return systemFunctionName.ToUpper();
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
            return GetFullyResolvedName();
        }
    }
}
