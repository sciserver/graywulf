using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class CommonTableExpression
    {
        #region Private member variables

        private Dictionary<string, TableReference> commonTableReferences;

        #endregion
        #region Properties

        public Dictionary<string, TableReference> CommonTableReferences
        {
            get { return commonTableReferences; }
        }

        #endregion
        #region Constructors and initializers

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.commonTableReferences = new Dictionary<string, TableReference>();
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (CommonTableExpression)other;

            this.commonTableReferences = new Dictionary<string, TableReference>();
        }

        public IEnumerable<CommonTableSpecification> EnumerateCommonTableSpecifications()
        {
            return this.EnumerateDescendantsRecursive<CommonTableSpecification>();
        }

        #endregion
    }
}
