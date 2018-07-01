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
        #endregion
        #region Properties

        public string FunctionName
        {
            get { return DatabaseObjectName; }
            set { DatabaseObjectName = value; }
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
                    return DatabaseObjectName;
                }
            }
        }

        #endregion
        #region Constructors and initializers

        public FunctionReference()
        {
            InitializeMembers();
        }

        public FunctionReference(Node node)
            :base(node)
        {
            InitializeMembers();
        }

        public FunctionReference(FunctionReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(FunctionReference old)
        {
        }

        public override object Clone()
        {
            return new FunctionReference(this);
        }

        #endregion

        public static FunctionReference Interpret(UdfIdentifier fi)
        {
            var ds = fi.FindDescendant<DatasetPrefix>();
            var fpi = fi.FindDescendant<FourPartIdentifier>();

            var fr = new FunctionReference(fi)
            {
                DatasetName = Util.RemoveIdentifierQuotes(ds?.DatasetName),
                DatabaseName = Util.RemoveIdentifierQuotes(fpi.NamePart3),
                SchemaName = Util.RemoveIdentifierQuotes(fpi.NamePart2),
                DatabaseObjectName = Util.RemoveIdentifierQuotes(fpi.NamePart1),
                IsUserDefined = true
            };

            // This is a system function call
            if (fr.DatasetName == null && fr.DatabaseName == null && fr.SchemaName == null)
            {
                fr.IsUserDefined = false;
            }

            return fr;
        }

        public static FunctionReference Interpret(UdtVariableMethodIdentifier fi)
        {
            // TODO
            throw new NotImplementedException();
        }

        public static FunctionReference Interpret(UdtStaticMethodIdentifier fi)
        {
            // TODO
            throw new NotImplementedException();
        }

        /* TODO: delete
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
        */

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
