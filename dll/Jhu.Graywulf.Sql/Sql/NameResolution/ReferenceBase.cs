using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public abstract class ReferenceBase : ICloneable
    {
        #region Property storage variables

        private Node node;
        private bool isResolved;
        private bool isUserDefined;

        #endregion
        #region Properties

        /// <summary>
        /// Gets the parser tree node this table reference references
        /// </summary>
        public Node Node
        {
            get { return node; }
            protected set { node = value; }
        }

        /// <summary>
        /// Gets or sets whether the name resolver has visited this node. It is somewhat independent
        /// of the databaseObject field != null because certain node do not directly reference
        /// database objects, such as subqueries.
        /// </summary>
        public bool IsResolved
        {
            get { return isResolved; }
            set { isResolved = value; }
        }

        /// <summary>
        /// Gets or sets whether the object is defined by the system or the user.
        /// </summary>
        /// <remarks>
        /// This is used to distinguish UDTs and UDFs and not user-defined tables.
        /// </remarks>
        public bool IsUserDefined
        {
            get { return isUserDefined; }
            set { isUserDefined = value; }
        }

        public bool IsSystem
        {
            get { return !isUserDefined; }
            set { isUserDefined = !value; }
        }

        public abstract string UniqueName { get; set; }

        #endregion
        #region Constructor and initializers

        protected ReferenceBase()
        {
            InitializeMembers();
        }

        protected ReferenceBase(Node node)
            : this()
        {
            this.node = node;
        }

        protected ReferenceBase(ReferenceBase old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.node = null;
            this.isResolved = false;
            this.isUserDefined = false;
            
        }

        private void CopyMembers(ReferenceBase old)
        {
            this.node = old.node;
            this.isResolved = old.isResolved;
            this.isUserDefined = old.isUserDefined;
        }

        public abstract object Clone();

        #endregion
    }
}
