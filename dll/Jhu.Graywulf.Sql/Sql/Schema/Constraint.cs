using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Sql.Schema
{
    public class Constraint : DatabaseObject
    {
        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        [IgnoreDataMember]
        public string ConstraintName
        {
            get { return ObjectName; }
            set { ObjectName = value; }
        }

        #region Constructors and initializers

        /// <summary>
        /// Creates a new index object
        /// </summary>
        public Constraint()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public Constraint(Constraint old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their default values
        /// </summary>
        [OnSerializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.ObjectType = DatabaseObjectType.Constraint;
        }

        private void CopyMembers(Constraint old)
        {
            this.ObjectType = old.ObjectType;
        }

        /// <summary>
        /// Returns a copy of this index
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Constraint(this);
        }

        #endregion
    }
}
