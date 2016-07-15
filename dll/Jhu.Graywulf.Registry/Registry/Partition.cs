/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Partition</b> entity
    /// </summary>
    public partial class Partition : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        protected long from;
        protected long to;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.Partition; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Federation; }
        }

        /// <summary>
        /// Lower limit of the interval of the <b>Partitioning Column</b> values belonging to this <b>Partition</b>.
        /// </summary>
        [DBColumn]
        public long From
        {
            get { return from; }
            set { from = value; }
        }

        /// <summary>
        /// Upper limit of the interval of the <b>Partitioning Column</b> values belonging to this <b>Partition</b>.
        /// </summary>
        [DBColumn]
        public long To
        {
            get { return to; }
            set { to = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Slice</b> object to which this <b>Partition</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public Slice Slice
        {
            get { return (Slice)ParentReference.Value; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public Partition()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Partition</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public Partition(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public Partition(Slice parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Partition</b> objects.
        /// </summary>
        /// <param name="old">The <b>Partition</b> to copy from.</param>
        public Partition(Partition old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their initial values.
        /// </summary>
        /// <remarks>
        /// This function is called by the contructors.
        /// </remarks>
        private void InitializeMembers()
        {
            this.from = 0;
            this.to = 0;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Partition</b> object to create the deep copy from.</param>
        private void CopyMembers(Partition old)
        {
            this.from = old.from;
            this.to = old.to;
        }

        internal override bool CompareMembers(Entity other)
        {
            bool eq = base.CompareMembers(other);
            var o = other as Partition;

            eq &= this.from == o.from;
            eq &= this.to == o.to;

            return eq;
        }

        internal override void UpdateMembers(Entity other)
        {
            base.UpdateMembers(other);
            var o = other as Partition;
            CopyMembers(o);
        }

        public override object Clone()
        {
            return new Partition(this);
        }

        #endregion
    }
}
