using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class FunctionReference : DatabaseObjectReference
    {
        #region Property storage variables

        private string systemFunctionName;

        private bool isUdf;

        #endregion

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
        /// 
        /// </summary>
        /// <remarks>
        /// Never use this in query generation!
        /// </remarks>
        public override string UniqueName
        {
            get
            {
                if (!IsSystem)
                {
                    return base.UniqueName;
                }
                else
                {
                    return systemFunctionName;
                }
            }
        }

        public FunctionReference()
        {
            InitializeMembers();
        }

        public FunctionReference(FunctionReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        public FunctionReference(string functionName)
        {
            InitializeMembers();
            InterpretSystemFunction(functionName);
        }

        public FunctionReference(FunctionName fn)
        {
            InitializeMembers();
            InterpretFunctionName(fn);
        }

        public FunctionReference(UdfIdentifier fi)
        {
            InitializeMembers();
            InterpretUdfIdentifier(fi);
        }

        private void InitializeMembers()
        {
            this.systemFunctionName = null;
            this.isUdf = true;
        }

        private void CopyMembers(FunctionReference old)
        {
            this.systemFunctionName = old.systemFunctionName;
            this.isUdf = old.isUdf;
        }

        private void InterpretSystemFunction(string functionName)
        {
            DatasetName = null;
            DatabaseName = null;
            SchemaName = null;
            DatabaseObjectName = null;

            systemFunctionName = functionName;

            isUdf = false;
        }

        private void InterpretFunctionName(FunctionName fn)
        {
            InterpretSystemFunction(fn.Value);
        }

        private void InterpretUdfIdentifier(UdfIdentifier udfi)
        {
            var ds = udfi.FindDescendant<DatasetName>();
            DatasetName = (ds != null) ? Util.RemoveIdentifierQuotes(ds.Value) : null;

            var dbn = udfi.FindDescendant<DatabaseName>();
            DatabaseName = (dbn != null) ? Util.RemoveIdentifierQuotes(dbn.Value) : null;

            var sn = udfi.FindDescendant<SchemaName>();
            SchemaName = (sn != null) ? Util.RemoveIdentifierQuotes(sn.Value) : null;

            var tn = udfi.FindDescendant<FunctionName>();
            DatabaseObjectName = (tn != null) ? Util.RemoveIdentifierQuotes(tn.Value) : null;

            systemFunctionName = null;

            isUdf = true;
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
    }
}
