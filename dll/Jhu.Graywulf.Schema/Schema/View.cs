using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Contains information about a database view
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class View : TableOrView, ICloneable
    {
        /// <summary>
        /// Gets or sets the name of the view
        /// </summary>
        public string ViewName
        {
            get { return ObjectName; }
            set { ObjectName = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public View()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Creates a view and initializes its dataset
        /// </summary>
        /// <param name="dataset"></param>
        public View(DatasetBase dataset)
            : base(dataset)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public View(View old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their default values
        /// </summary>
        private void InitializeMembers()
        {
            this.ObjectType = DatabaseObjectType.View;
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(View old)
        {
            this.ObjectType = DatabaseObjectType.View;
        }

        /// <summary>
        /// Returns a copy of this view
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new View(this);
        }
    }
}
