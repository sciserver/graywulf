using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Acts as the base class of all object requiring a context.
    /// </summary>
    public abstract class ContextObject : IRegistryContextObject
    {
        #region Member Variables

        private RegistryContext context;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the context of the object.
        /// </summary>
        [XmlIgnore]
        public RegistryContext RegistryContext
        {
            get { return context; }
            set { context = value; }
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor that initializes private members to their
        /// defaul values.
        /// </summary>
        public ContextObject()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor that creates an objects with a context set.
        /// </summary>
        /// <param name="context"></param>
        public ContextObject(RegistryContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Copy constructor that creates a deep copy from the passed object.
        /// </summary>
        /// <param name="old"></param>
        public ContextObject(ContextObject old)
        {
            CopyMembers(old);
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes private members to their default values.
        /// </summary>
        private void InitializeMembers()
        {
            this.context = null;
        }

        /// <summary>
        /// Copies private members from the passed object.
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(ContextObject old)
        {
            this.context = old.context;
        }

        #endregion
    }
}
