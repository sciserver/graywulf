using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class QueryDetails
    {
        #region Private member variables

        private StatementBlock parsingTree;

        private bool isResolved;

        private bool isPartitioned;

        /// <summary>
        /// Name of the table used for partitioning
        /// </summary>
        [NonSerialized]
        private TableReference partitioningTable;

        /// <summary>
        /// Name of the column to partition on
        /// </summary>
        [NonSerialized]
        private ColumnReference partitioningKey;

        // TODO: key these with object instead of unique name?
        //       the schema objects should be the same since they're cached

        private Dictionary<string, VariableReference> variableReferences;
        private Dictionary<string, List<TableReference>> sourceTableReferences;
        private Dictionary<string, List<TableReference>> outputTableReferences;

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

        public Dictionary<string, VariableReference> VariableReferences
        {
            get { return variableReferences; }
        }

        public Dictionary<string, List<TableReference>> SourceTables
        {
            get { return sourceTableReferences; }
        }

        public Dictionary<string, List<TableReference>> OutputTables
        {
            get { return outputTableReferences; }
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
            this.sourceTableReferences = new Dictionary<string, List<TableReference>>(Schema.SchemaManager.Comparer);
            this.outputTableReferences = new Dictionary<string, List<TableReference>>(Schema.SchemaManager.Comparer);
        }

        private void CopyMembers(QueryDetails old)
        {
            this.parsingTree = new StatementBlock(old.ParsingTree);
            this.isResolved = old.isResolved;
            this.isPartitioned = old.isPartitioned;
            this.partitioningTable = old.partitioningTable;
            this.partitioningKey = old.partitioningKey;
            this.variableReferences = new Dictionary<string, VariableReference>(old.variableReferences);
            this.sourceTableReferences = new Dictionary<string, List<TableReference>>(old.sourceTableReferences);
            this.outputTableReferences = new Dictionary<string, List<TableReference>>(old.outputTableReferences);
        }

        #endregion
    }
}
