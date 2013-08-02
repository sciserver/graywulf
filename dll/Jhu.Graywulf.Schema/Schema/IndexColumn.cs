using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Reflects an index column
    /// </summary>
    [Serializable]
    public class IndexColumn : Column, ICloneable
    {
        private int keyOrdinal;
        private IndexColumnOrdering ordering;

        /// <summary>
        /// Gets or sets the ordinal position of the column within the index key
        /// </summary>
        public int KeyOrdinal
        {
            get { return keyOrdinal; }
            set { keyOrdinal = value; }
        }

        /// <summary>
        /// Gets or sets the ordering of the index column-
        /// </summary>
        public IndexColumnOrdering Ordering
        {
            get { return ordering; }
            set { ordering = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public IndexColumn()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Creates a new index column and sets its parent index
        /// </summary>
        public IndexColumn(DatabaseObject parent)
            :base(parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public IndexColumn(IndexColumn old)
            : base(old)
        {
            CopyMembers(old);
        }

        // <summary>
        /// Initializes member variables
        /// </summary>
        private void InitializeMembers()
        {
            this.keyOrdinal = -1;
            this.ordering = IndexColumnOrdering.Unknown;
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(IndexColumn old)
        {
            this.keyOrdinal = old.keyOrdinal;
            this.ordering = old.ordering;
        }

        #region ICloneable Members

        /// <summary>
        /// Returns a copy of the index column
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new IndexColumn(this);
        }

        #endregion
    }
}
