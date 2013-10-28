using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Types;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Reflects a scalar function
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class ScalarFunction : DatabaseObject, IParameters, ICloneable
    {
        private LazyProperty<ConcurrentDictionary<string, Parameter>> parameters;

        /// <summary>
        /// Gets or sets the name of the function
        /// </summary>
        public string FunctionName
        {
            get { return ObjectName; }
            set { ObjectName = value; }
        }

        /// <summary>
        /// Gets the return type of the function
        /// </summary>
        public DataType ReturnType
        {
            get
            {
                return Parameters.Values.First(p => p.Direction == ParameterDirection.ReturnValue).DataType;
            }
        }

        /// <summary>
        /// Gets the parameter collection
        /// </summary>
        public ConcurrentDictionary<string, Parameter> Parameters
        {
            get
            {
                return parameters.Value;
            }
        }

        #region Constructors and initializers

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScalarFunction()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Creates a scalar function and initializes its
        /// dataset
        /// </summary>
        /// <param name="dataset"></param>
        public ScalarFunction(DatasetBase dataset)
            : base(dataset)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public ScalarFunction(ScalarFunction old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables
        /// </summary>
        private void InitializeMembers()
        {
            this.ObjectType = DatabaseObjectType.ScalarFunction;
            this.parameters = new LazyProperty<ConcurrentDictionary<string, Parameter>>(LoadParameters);
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(ScalarFunction old)
        {
            this.parameters = new LazyProperty<ConcurrentDictionary<string, Parameter>>(LoadParameters);
        }

        /// <summary>
        /// Returns a copy of this scalar function
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return new ScalarFunction(this);
        }

        #endregion
    }
}
