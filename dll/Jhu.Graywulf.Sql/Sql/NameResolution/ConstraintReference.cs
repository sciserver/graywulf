using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class ConstraintReference : DatabaseObjectReference
    {
        #region Property storage variables

        #endregion

        #region Properties

        public string ConstraintName
        {
            get { return DatabaseObjectName; }
            set { DatabaseObjectName = value; }
        }

        public Constraint Constraint
        {
            get { return (Constraint)DatabaseObject; }
            set { DatabaseObject = value; }
        }

        public override string UniqueName
        {
            get { return DatabaseObjectName; }
        }

        #endregion
        #region Constructors and initializer

        public ConstraintReference()
        {
            InitializeMembers();
        }

        public ConstraintReference(Node node)
            : base(node)
        {
            InitializeMembers();
        }
        
        public ConstraintReference(Constraint constraint)
            : base(constraint)
        {
            InitializeMembers();

            throw new NotImplementedException();
        }

        public ConstraintReference(ConstraintReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(ConstraintReference old)
        {
        }

        public override object Clone()
        {
            return new ConstraintReference(this);
        }

        #endregion

        public static ConstraintReference Interpret(ConstraintName cn)
        {
            var cr = new ConstraintReference()
            {
                ConstraintName = RemoveIdentifierQuotes(cn?.Value),
            };

            return cr;
        }
    }
}
