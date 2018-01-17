using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class StatementBlock : IStatement
    {
        #region Private member variables

        private Dictionary<string, VariableReference> variableReferences;
        private Dictionary<string, TableReference> sourceTableReferences;

        #endregion
        #region Properties

        public bool IsResolvable
        {
            get { return false; }
        }

        public Dictionary<string, VariableReference> VariableReferences
        {
            get { return variableReferences; }
        }

        public Dictionary<string, TableReference> SourceTableReferences
        {
            get { return sourceTableReferences; }
        }

        #endregion
        #region Constructors and initializers

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.variableReferences = new Dictionary<string, VariableReference>(Schema.SchemaManager.Comparer);
            this.sourceTableReferences = new Dictionary<string, TableReference>(Schema.SchemaManager.Comparer);
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (StatementBlock)other;

            this.variableReferences = new Dictionary<string, VariableReference>(old.variableReferences);
            this.sourceTableReferences = new Dictionary<string, TableReference>(old.sourceTableReferences);
        }

        #endregion

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            return EnumerateDescendants<Statement>(true);
        }
    }
}
