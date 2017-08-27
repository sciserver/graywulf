using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdfIdentifier : IFunctionReference
    {
        private FunctionReference functionReference;

        public FunctionReference FunctionReference
        {
            get { return functionReference; }
            set { functionReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.functionReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (UdfIdentifier)other;

            this.functionReference = old.functionReference;
        }

        public static UdfIdentifier Create()
        {
            var udfi = new UdfIdentifier();
            return udfi;
        }

    }
}
