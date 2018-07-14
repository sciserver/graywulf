using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ConstraintName : IConstraintReference
    {
        private ConstraintReference constraintReference;

        public ConstraintReference ConstraintReference
        {
            get { return constraintReference; }
            set { constraintReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.constraintReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (ConstraintName)other;
            this.constraintReference = old.constraintReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.constraintReference = ConstraintReference.Interpret(this);
        }
    }
}
