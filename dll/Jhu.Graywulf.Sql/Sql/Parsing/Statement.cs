using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public abstract partial class Statement : ISourceTableProvider
    {
        #region Private member variables

        private Dictionary<string, TableReference> sourceTableReferences;

        #endregion

        public Dictionary<string, TableReference> SourceTableReferences
        {
            get { return sourceTableReferences; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.sourceTableReferences = new Dictionary<string, TableReference>(Schema.SchemaManager.Comparer);
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (Statement)other;
            this.sourceTableReferences = new Dictionary<string, TableReference>(old.sourceTableReferences);
        }

        public virtual IEnumerable<Statement> EnumerateSubStatements()
        {
            yield break;
        }
    }
}
