using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class IndexName : IIndexReference
    {
        private IndexReference indexReference;

        public IndexReference IndexReference
        {
            get { return indexReference; }
            set { indexReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.indexReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (IndexName)other;
            this.indexReference = old.indexReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.indexReference = IndexReference.Interpret(this);
        }
    }
}
