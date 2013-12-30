using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Reflects a stored procedure
    /// </summary>
    [Serializable]
    public class StoredProcedure : DatabaseObject, IParameters, ICloneable
    {
        private LazyProperty<ConcurrentDictionary<string, Parameter>> parameters;

        /// <summary>
        /// Gets or sets the name of the stored procedure
        /// </summary>
        public string ProcedureName
        {
            get { return ObjectName; }
            set { ObjectName = value; }
        }

        /// <summary>
        /// Gets the parameter collection
        /// </summary>
        public ConcurrentDictionary<string, Parameter> Parameters
        {
            get { return parameters.Value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public StoredProcedure()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Creates a stored procedure function and initializes its dataset
        /// </summary>
        /// <param name="dataset"></param>
        public StoredProcedure(DatasetBase dataset)
            : base(dataset)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public StoredProcedure(StoredProcedure old)
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
            this.ObjectType = DatabaseObjectType.StoredProcedure;
            this.parameters = new LazyProperty<ConcurrentDictionary<string, Parameter>>(LoadParameters);
        }

        /// <summary>
        /// Copies member variables.
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(StoredProcedure old)
        {
            this.ObjectType = old.ObjectType;
            this.parameters = new LazyProperty<ConcurrentDictionary<string, Parameter>>(LoadParameters);
        }

        #region ICloneable Members

        /// <summary>
        /// Returns a copy of this stored procedure
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return new StoredProcedure(this);
        }

        #endregion
    }
}
