using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Reflects an index column
    /// </summary>
    [Serializable]
    [DataContract(Namespace="")]
    public class IndexColumn : Column, ICloneable
    {
        [NonSerialized]
        private bool isIncluded;

        [NonSerialized]
        private int keyOrdinal;

        [NonSerialized]
        private IndexColumnOrdering ordering;


        [DataMember]
        public bool IsIncluded
        {
            get { return isIncluded; }
            set { isIncluded = value; }
        }

        /// <summary>
        /// Gets or sets the ordinal position of the column within the index key
        /// </summary>
        [DataMember]
        public int KeyOrdinal
        {
            get { return keyOrdinal; }
            set { keyOrdinal = value; }
        }

        /// <summary>
        /// Gets or sets the ordering of the index column-
        /// </summary>
        [DataMember]
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
            this.isIncluded = false;
            this.keyOrdinal = -1;
            this.ordering = IndexColumnOrdering.Unknown;
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(IndexColumn old)
        {
            this.isIncluded = old.isIncluded;
            this.keyOrdinal = old.keyOrdinal;
            this.ordering = old.ordering;
        }

        /// <summary>
        /// Returns a copy of the index column
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new IndexColumn(this);
        }
    }
}
