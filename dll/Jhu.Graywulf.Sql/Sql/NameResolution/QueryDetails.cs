using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class QueryDetails
    {
        #region Private member variables

        private StatementBlock parsingTree;
        private bool isResolved;
        private bool isPartitioned;

        /// <summary>
        /// Name of the table used for partitioning
        /// </summary>
        private TableReference partitioningTable;

        /// <summary>
        /// Name of the column to partition on
        /// </summary>
        private ColumnReference partitioningKey;

        // TODO: key these with object instead of unique name?
        //       the schema objects should be the same since they're cached

        private Dictionary<string, VariableReference> variableReferences;
        private Dictionary<string, DataTypeReference> dataTypeReferences;
        private Dictionary<string, List<TableReference>> sourceTableReferences;
        private Dictionary<string, List<TableReference>> targetTableReferences;
        private Dictionary<string, List<TableReference>> outputTableReferences;
        private Dictionary<string, List<FunctionReference>> functionReferences;

        #endregion
        #region Properties

        public StatementBlock ParsingTree
        {
            get { return parsingTree; }
            set { parsingTree = value; }
        }

        public bool IsResolved
        {
            get { return isResolved; }
            set { isResolved = value; }
        }

        public bool IsPartitioned
        {
            get { return isPartitioned; }
            set { isPartitioned = value; }
        }

        /// <summary>
        /// A collection of all variables defined by the SQL script.
        /// </summary>
        public Dictionary<string, VariableReference> VariableReferences
        {
            get { return variableReferences; }
        }

        /// <summary>
        /// A collection of data type referenced by the query
        /// </summary>
        public Dictionary<string, DataTypeReference> DataTypeReferences
        {
            get { return dataTypeReferences; }
        }

        /// <summary>
        /// A collection of all tables and view used by the script as input
        /// </summary>
        public Dictionary<string, List<TableReference>> SourceTableReferences
        {
            get { return sourceTableReferences; }
        }

        /// <summary>
        /// A collection of all tables referenced by DML operations as the
        /// target table. This includes UPDATE, DELETE and INSERT, as well as
        /// MODIFY TABLE, DROP TABLE, etc.
        /// </summary>
        public Dictionary<string, List<TableReference>> TargetTableReferences
        {
            get { return targetTableReferences; }
        }

        /// <summary>
        /// A collection of all tables defined by the script as output tables.
        /// This includes SELECT INTO and CREATE TABLE
        /// </summary>
        public Dictionary<string, List<TableReference>> OutputTableReferences
        {
            get { return outputTableReferences; }
        }

        /// <summary>
        /// A collection of all functions referenced by the script.
        /// </summary>
        public Dictionary<string, List<FunctionReference>> FunctionReferences
        {
            get { return functionReferences; }
        }

        #endregion
        #region Constructors and initializers

        public QueryDetails()
        {
            InitializeMembers();
        }

        public QueryDetails(QueryDetails old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.parsingTree = null;
            this.isResolved = false;
            this.isPartitioned = false;
            this.partitioningTable = null;
            this.partitioningKey = null;
            this.variableReferences = new Dictionary<string, VariableReference>(Schema.SchemaManager.Comparer);
            this.dataTypeReferences = new Dictionary<string, DataTypeReference>(Schema.SchemaManager.Comparer);
            this.sourceTableReferences = new Dictionary<string, List<TableReference>>(Schema.SchemaManager.Comparer);
            this.targetTableReferences = new Dictionary<string, List<TableReference>>(Schema.SchemaManager.Comparer);
            this.outputTableReferences = new Dictionary<string, List<TableReference>>(Schema.SchemaManager.Comparer);
            this.functionReferences = new Dictionary<string, List<FunctionReference>>(Schema.SchemaManager.Comparer);
        }

        private void CopyMembers(QueryDetails old)
        {
            this.parsingTree = new StatementBlock(old.ParsingTree);
            this.isResolved = old.isResolved;
            this.isPartitioned = old.isPartitioned;
            this.partitioningTable = old.partitioningTable;
            this.partitioningKey = old.partitioningKey;
            this.variableReferences = new Dictionary<string, VariableReference>(old.variableReferences);
            this.dataTypeReferences = new Dictionary<string, DataTypeReference>(old.dataTypeReferences);
            this.sourceTableReferences = new Dictionary<string, List<TableReference>>(old.sourceTableReferences);
            this.targetTableReferences = new Dictionary<string, List<TableReference>>(old.targetTableReferences);
            this.outputTableReferences = new Dictionary<string, List<TableReference>>(old.outputTableReferences);
            this.functionReferences = new Dictionary<string, List<FunctionReference>>(old.functionReferences);
        }

        #endregion
    }
}
