using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Sql.Schema
{
    /// <summary>
    /// Reflects a scalar function
    /// </summary>
    [Serializable]
    [DataContract(Namespace="")]
    public class AggregateFunction : ScalarFunction, IParameters, ICloneable
    {
        #region Constructors and initializers

        /// <summary>
        /// Default constructor
        /// </summary>
        public AggregateFunction()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Creates a scalar function and initializes its
        /// dataset
        /// </summary>
        /// <param name="dataset"></param>
        public AggregateFunction(DatasetBase dataset)
            : base(dataset)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public AggregateFunction(ScalarFunction old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables
        /// </summary>
        [OnSerializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.ObjectType = DatabaseObjectType.AggregateFunction;
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(ScalarFunction old)
        {
        }

        /// <summary>
        /// Returns a copy of this scalar function
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return new AggregateFunction(this);
        }

        #endregion
    }
}
