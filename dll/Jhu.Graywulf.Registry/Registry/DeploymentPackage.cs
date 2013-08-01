/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Deployment Package</b> entity
    /// </summary>
    public partial class DeploymentPackage : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private string filename;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the name of the file stored in the associated binary blob.
        /// </summary>
        /// <remarks>
        /// The associated binary blob can be read and written using the <see cref="GetData"/>
        /// and <see cref="SetData"/> functions.
        /// </remarks>
        [DBColumn(Size = 128)]
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Database Definition</b> object to which this <b>Deployment Package</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public DatabaseDefinition DatabaseDefinition
        {
            get { return (DatabaseDefinition)ParentReference.Value; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public DeploymentPackage()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Deployment Package</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public DeploymentPackage(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public DeploymentPackage(DatabaseDefinition parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Deployment Package</b> objects.
        /// </summary>
        /// <param name="old">The <b>Federation</b> to copy from.</param>
        public DeploymentPackage(DeploymentPackage old)
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
            base.EntityType = EntityType.DeploymentPackage;
            base.EntityGroup = EntityGroup.Federation;

            this.filename = string.Empty;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Deployment Package</b> object to create the deep copy from.</param>
        private void CopyMembers(DeploymentPackage old)
        {
            this.filename = old.filename;
        }

        public override object Clone()
        {
            return new DeploymentPackage(this);
        }

        #endregion
    }
}
